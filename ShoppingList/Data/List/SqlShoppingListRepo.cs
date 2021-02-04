using System;
using System.Linq;
using System.Runtime.CompilerServices;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using SharedTypes.Entities;
using ShoppingData;
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
    public class SqlShoppingListRepo : IShoppingListRepo
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

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> CreateShoppingList(
            Option<ShoppingListCreateDto> shoppingList) =>
            Try(() =>
                    shoppingList
                        .Map
                        (i =>
                        {
                            return ValidateShopName2(
                                    _waypointsRepo.GetShopWaypointsId, i)
                                .Map(i =>
                                {
                                    _context.Database.CloseConnection();
                                    return _context.ShoppingListEntities.Add(i.Item1).Entity;
                                });
                        }))
                .TryOptionEitherMap(i => SaveChanges() ? Some(i) : new None())
                .TryOptionMap(i =>
                    i.NoneToEitherLeft(ShoppingListErrors.ShoppingListErrors.NewOtherError(new SavingFailed())))
                .TryOptionEitherMap(i => (ShoppingListReadDto) i)
                .Run()
                .Match(l =>
                        Left(ShoppingListErrors.ShoppingListErrors.NewOtherError(new SavingFailed())),
                    r => r.Map(i =>
                            i.Match(l =>
                                    (Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>)
                                    Left(l),
                                r =>
                                    (Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>)
                                    Right(r)
                            ))
                        .GetOrElse(
                            (Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>)
                            ShoppingListErrors.ShoppingListErrors.NewOtherError(new SavingFailed()))
                );

        private Func<ShoppingListCreateDto,
            Either<ShoppingListErrors.ShoppingListErrors, (ShoppingListCreateDto, Option<int>)>> ValidateShopName =>
            ValidateShopName2
                .Apply(_waypointsRepo.GetShopWaypointsId);

        internal static readonly
            Func<Func<string, Option<int>>,
                ShoppingListCreateDto,
                Either<ShoppingListErrors.ShoppingListErrors, (ShoppingListCreateDto, Option<int>)>>
            ValidateShopName2 = (getWaypointsId, createDto) =>
                createDto.ShopName.Length == 0
                    // empty ShopName is allowed
                    ? (Either<ShoppingListErrors.ShoppingListErrors, (ShoppingListCreateDto, Option<int>)>)
                    Right((createDto, new Option<int>()))
                    : getWaypointsId(createDto.ShopName)
                        .Map(i => (createDto, Some(i))).Match(
                            () =>
                                (Either<ShoppingListErrors.ShoppingListErrors, (ShoppingListCreateDto, Option<int>
                                    waypointsId)>)
                                Left(ShoppingListErrors.ShoppingListErrors.ShopNotFound),
                            some => Right(some)
                        );

        private Option<ShoppingListEntity> GetShoppingListEntityById(int id) =>
            _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id)
                .Pipe(i => (Option<ShoppingListEntity>) i!);

        private static Func<T1, Either<ShoppingListErrors.ShoppingListErrors, T2>> WrapEither<T1, T2>
            (Func<T1, T2> f) => input =>
            Try(() => f(input))
                .Run()
                .Match(l =>
                        (Either<ShoppingListErrors.ShoppingListErrors, T2>)
                        Left(ShoppingListErrors.ShoppingListErrors.NewOtherError(new OtherError(nameof(l)))),
                    r => Right(r));

        private Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity> EitherGetShoppingListEntity(int id) =>
            WrapEither<int, Option<ShoppingListEntity>>(GetShoppingListEntityById)
                .Map(i => i.NoneToEitherLeft(ShoppingListErrors.ShoppingListErrors.NotFound))(id);

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>
            GetShoppingList(int id) =>
            EitherGetShoppingListEntity(id)
                .Map(j => (ShoppingListReadDto) j);

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>
            GetShoppingListSorted(int id) =>
            WrapEither<int, Option<ShoppingListEntity>>(GetShoppingListEntityById)
                .Map(i => i.EitherOptionBind(SortEntity))
                .Map(r => r.NoneToEitherLeft(ShoppingListErrors.ShoppingListErrors.NotFound))
                .Map(i => i.Map(j => (ShoppingListReadDto) j))(id);

        private Option<ShoppingListEntity> SortEntity(ShoppingListEntity entity) =>
            ShouldTryFindWaypoints(entity)
                .Map(k =>
                    MatchWaypointsToShoppingList(k, _waypointsRepo.GetShopWaypoints)
                        .Map(SortToOptimalOrder)
                        .Map(i => (ShoppingListEntity) i)
                )
                .Match(() => entity, r =>
                    r.Match(_ =>
                    {
                        Console.WriteLine($"Waypoints of requested shop: {entity.ShopName} was not found");
                        return entity;
                    }, right => right)
                );

        internal static Option<ShoppingListEntity>
            ShouldTryFindWaypoints(Option<ShoppingListEntity> shoppingListEntity) =>
            shoppingListEntity.Bind(i =>
                i.ShopName.Length > 0 ? Some(i) : new None());

        internal static Either<ShopWaypointsNotFound, (ShopWaypointsReadDto, ShoppingListModule.ShoppingList)>
            MatchWaypointsToShoppingList(
                ShoppingListEntity shoppingListEntity,
                Func<string, Option<ShopWaypointsReadDto>> getShopWaypoints)
            =>
                getShopWaypoints(shoppingListEntity.ShopName)
                    .Map(waypointsDto =>
                        (Either<ShopWaypointsNotFound, (ShopWaypointsReadDto, ShoppingListModule.ShoppingList)>)
                        (waypointsDto, (ShoppingListModule.ShoppingList) shoppingListEntity)
                    )
                    .GetOrElse(new ShopWaypointsNotFound());

        private ShoppingListModule.ShoppingList SortToOptimalOrder(
            (ShopWaypointsReadDto, ShoppingListModule.ShoppingList) listWithWaypoints) =>
            WaypointsOrder.sortShoppingList
            (FuncConvert.FromFunc(PredictionAdapter.PredictionFunc.Apply(_predictionEnginePool)),
                listWithWaypoints.Item1, listWithWaypoints.Item2);

        public Either<ShoppingListErrors.ShoppingListErrors, int> PasswordMatchesShoppingList(int shoppingListId,
            string password) =>
            EitherGetShoppingListEntity(shoppingListId)
                .Bind<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity, int>(i =>
                {
                    if (i.Password == password) return Right(i.Id);
                    else return Left(ShoppingListErrors.ShoppingListErrors.IncorrectPassword);
                });

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> AddItemToShoppingListDto(
            Option<ItemDataCreateDto> itemToAdd) =>
            itemToAdd
                .Pipe(AddItemToShoppingList)
                .Map(i => (ShoppingListReadDto) i);

        private Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity> AddItemToShoppingList(
            Option<ItemDataCreateDto> itemToAdd) =>
            itemToAdd
                .Pipe(AddItemToList)
                .Map(WrapSaving)
                .OptionTryEitherMap(SortEntity, ShoppingListErrors.ShoppingListErrors.NewOtherError(new UnknownError()))
                .Map(RunSaving)
                .GetOrElse((Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>) ShoppingListErrors
                    .ShoppingListErrors.NewOtherError(new UnknownError()));

        private Func<Option<ItemDataCreateDto>,
            Option<(ShoppingListEntity shoppingListEntity, ShoppingListModule.ShoppingList result)>> AddItemToList =>
            AddItemToList2.Apply(GetShoppingListWithChildrenById);

        internal static readonly Func<Func<int, Option<ShoppingListEntity>>, Option<ItemDataCreateDto>, Option<(
            ShoppingListEntity
            shoppingListEntity, ShoppingListModule.ShoppingList result)>> AddItemToList2 =
            (shoppingListWithChildrenById, itemToAdd) =>
                itemToAdd
                    .Bind(i =>
                        shoppingListWithChildrenById(i.ShoppingListId)
                            .Map(dbList => (i, dbList)))
                    .Map(pair =>
                    {
                        var (itemDataCreateDto, shoppingListEntity) = pair;
                        var result = addItem(shoppingListEntity, (ItemDataEntity) itemDataCreateDto);
                        return (shoppingListEntity, result);
                    });

        private Try<Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>> SortTryEither(
            Try<Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>> entityToSort)
            => entityToSort
                .Map(j =>
                    j.Bind
                    (k => SortEntity(k)
                        .Map(m => (Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>) Right(m))
                        .GetOrElse(() => Left(ShoppingListErrors.ShoppingListErrors.NewOtherError(new UnknownError())))
                    ));

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

        private Option<ShoppingListEntity> GetShoppingListWithChildrenById(int id) =>
            (Option<ShoppingListEntity>) _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id)!;
    }
}