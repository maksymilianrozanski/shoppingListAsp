using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FSharpPlus.Control;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using SharedTypes.Entities;
using ShoppingData;
using ShoppingList.Auth;
using ShoppingList.Data.List.Errors;
using ShoppingList.Data.Waypoints;
using ShoppingList.Utils;
using ShoppingListSorting;
using static LaYumba.Functional.F;
using static ShoppingData.ShoppingListModule;
using GroceryPredictionPool = Microsoft.Extensions.ML.PredictionEnginePool<GroceryClassification.GroceryData,
    GroceryClassification.GroceryItemPrediction>;
using static ShoppingList.Utils.EitherUtils;

[assembly: InternalsVisibleTo("ShoppingList.Tests")]

namespace ShoppingList.Data.List
{
    public partial class SqlShoppingListRepo : IShoppingListRepo
    {
        private readonly ShoppingListDbContext _context;
        private readonly IWaypointsRepo _waypointsRepo;
        private readonly GroceryPredictionPool _predictionEnginePool;

        public SqlShoppingListRepo(ShoppingListDbContext context,
            GroceryPredictionPool predictionEnginePool)
        {
            _context = context;
            _waypointsRepo = new SqlWaypointsRepo(_context);
            _predictionEnginePool = predictionEnginePool;
        }

        private Either<ShoppingListErrors.ShoppingListErrors, (ShoppingListCreateDto, Option<ShopWaypointsEntity>)>
            MatchWaypoints(ShoppingListCreateDto createDto) =>
            GetWaypointsByName(_waypointsRepo.GetShopWaypointsEntity, createDto);

        private ShoppingListEntity AttachWaypointsEntity((ShoppingListCreateDto, Option<ShopWaypointsEntity>) i)
        {
            var (createDto, optionWaypoints) = i;
            ShoppingListEntity entityToAdd = createDto.ToShoppingListEntity(optionWaypoints);
            return _context.ShoppingListEntities.Add(entityToAdd).Entity;
        }

        internal static
            Either<ShoppingListErrors.ShoppingListErrors, (ShoppingListCreateDto, Option<T>)> GetWaypointsByName<T>(
                Func<string, Option<T>> getWaypointsId, ShoppingListCreateDto createDto) =>
            createDto.ShopName.Length == 0
                // empty ShopName is allowed
                ? Right((createDto, new Option<T>()))
                : getWaypointsId(createDto.ShopName)
                    .Map(i => (createDto, Some(i))).Match(
                        () =>
                            (Either<ShoppingListErrors.ShoppingListErrors, (ShoppingListCreateDto, Option<T>)>)
                            Left(ShoppingListErrors.ShoppingListErrors.ShopNotFound),
                        some => Right(some)
                    );

        private static Func<T1, Either<ShoppingListErrors.ShoppingListErrors, T2>> WrapEither<T1, T2>
            (Func<T1, T2> f) => input =>
            Try(() => f(input))
                .Run()
                .Match(l =>
                        (Either<ShoppingListErrors.ShoppingListErrors, T2>)
                        Left(ShoppingListErrors.ShoppingListErrors.NewOtherError(new OtherError(nameof(l)))),
                    r => Right(r));

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> AddItemToShoppingListDto(
            Option<ItemDataCreateDto> itemToAdd) =>
            itemToAdd
                .Pipe(AddItemToShoppingList)
                .Map(i => (ShoppingListReadDto) i);

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> ModifyShoppingList(
            Option<ItemDataActionDto> itemDataAction) =>
            itemDataAction
                .Pipe(ApplyItemDataAction)
                .Map(r =>
                    FSharpChoiceToEither(r.result)
                        .Map(i => (r.entityFromDb, i)))
                .OptionEitherMap(WrapSaving)
                .OptionEitherMap(SortTryEither)
                .OptionEitherMap(RunSaving)
                .Map(FlattenEither)
                .Map(i => i.Map(j => (ShoppingListReadDto) j))
                .GetOrElse(() => Left(ShoppingListErrors.ShoppingListErrors.NewOtherError(new UnknownError())));

        private Func<Option<ItemDataActionDto>, Option<(ShoppingListEntity entityFromDb,
                FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors> result)>>
            ApplyItemDataAction => ApplyItemDataAction2.Apply(GetShoppingListWithChildrenById);

        internal static readonly Func<Func<int, Option<ShoppingListEntity>>, Option<ItemDataActionDto>, Option<(
                ShoppingListEntity
                entityFromDb,
                FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors> result)>>
            ApplyItemDataAction2 =
                (getShoppingListWithChildrenById, itemDataAction) =>
                    itemDataAction.Bind(i => getShoppingListWithChildrenById(i.ShoppingListId)
                            .Map(j => (i, j).ToTuple()))
                        .Bind(bothNonEmpty =>
                        {
                            var (updateDto, entityFromDb) = bothNonEmpty;
                            return ItemDataActionDto.Actions.TryGetValue(
                                (ItemDataActionDto.ItemDataActions)
                                updateDto.ActionNumber, out var modifyingFunction)
                                ? Some((updateDto, entityFromDb, modifyingFunction).ToTuple())
                                : new None();
                        })
                        .Map(r =>
                        {
                            var (updateDto, entityFromDb, modifyingFunction) = r;
                            var result = modifyingFunction
                                .Invoke(updateDto.User)
                                .Invoke(updateDto.ItemId)
                                .Invoke(entityFromDb);
                            return (entityFromDb, result);
                        });

        private Try<Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>> WrapSaving(
            (ShoppingListEntity, ShoppingListModule.ShoppingList) values)
        {
            var (entityFromDb, result) = values;
            _context.Entry(entityFromDb).CurrentValues.SetValues(result);
            _context.ItemDataEntities.RemoveRange(entityFromDb.ItemDataEntities);
            result.Items.ToList()
                .ForEach(i => entityFromDb.ItemDataEntities.Add(i));
            return Try(SaveChanges)
                .Map
                    <bool, Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>>
                    (isSuccessful => isSuccessful
                        ? Right(entityFromDb)
                        : Left(ShoppingListErrors.ShoppingListErrors.NewOtherError(new SavingFailed())));
        }

        private Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity> RunSaving(
            Try<Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>> trySave) =>
            trySave
                .Run().Match(
                    l =>
                        (Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>)
                        Left(
                            ShoppingListErrors.ShoppingListErrors.NewOtherError(new UnknownError())),
                    r => r);

        public bool SaveChanges() => _context.SaveChanges() >= 0;
    }
}