using System;
using System.Linq;
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

namespace ShoppingList.Data.List
{
    public class SqlShoppingListRepo : IShoppingListRepo
    {
        private readonly ShoppingListDbContext _context;
        private readonly IWaypointsRepo _waypointsRepo;
        private readonly GroceryPredictionPool _predictionEnginePool;

        public SqlShoppingListRepo(ShoppingListDbContext context, IWaypointsRepo waypointsRepo,
            GroceryPredictionPool predictionEnginePool)
        {
            _context = context;
            _waypointsRepo = waypointsRepo;
            _predictionEnginePool = predictionEnginePool;
        }

        public Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList) =>
            shoppingList.Map(i => _context.ShoppingListEntities.Add(i).Entity)
                .Bind(i => SaveChanges() ? Some(i) : new None())
                .Map(i => (ShoppingListReadDto) i);

        private Option<ShoppingListEntity> GetShoppingListById(int id) =>
            _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id)
                .Pipe(i => (Option<ShoppingListEntity>) i!);

        public Option<ShoppingListReadDto> GetShoppingListReadDtoById(int id) =>
            GetShoppingListById(id)
                .Map(i => (ShoppingListReadDto) i);

        public Option<ShoppingListReadDto> GetShoppingListReadDtoByIdWithSorting(int id) =>
            GetShoppingListById(id)
                .Bind(SortEntity)
                .Map(i => (ShoppingListReadDto) i);

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

        private static Option<ShoppingListEntity>
            ShouldTryFindWaypoints(Option<ShoppingListEntity> shoppingListEntity) =>
            shoppingListEntity.Bind(i =>
                i.ShopName.Length > 0 ? Some(i) : new None());

        private static Either<ShopWaypointsNotFound, (ShopWaypointsReadDto, ShoppingListModule.ShoppingList)>
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
            GetShoppingListById(shoppingListId)
                .Map<ShoppingListEntity, Either<ShoppingListErrors.ShoppingListErrors, int>>(i =>
                {
                    if (i.Password == password) return Right(i.Id);
                    else return Left(ShoppingListErrors.ShoppingListErrors.IncorrectPassword);
                })
                .GetOrElse(Left(ShoppingListErrors.ShoppingListErrors.ListItemNotFound));

        public Either<Error, ShoppingListReadDto> AddItemToShoppingListDto(
            Option<ItemDataCreateDto> itemToAdd)
        {
            Console.WriteLine("received AddItemToShoppingListDto (without password)request");
            return itemToAdd
                .Pipe(AddItemToShoppingList)
                .Map(i => (ShoppingListReadDto) i);
        }

        private Either<Error, ShoppingListEntity> AddItemToShoppingList(
            Option<ItemDataCreateDto> itemToAdd)
        {
            Console.WriteLine("received AddItemToShoppingListDto (without password)request");
            return itemToAdd
                .Pipe(AddItemToList)
                .Map(WrapSaving)
                .OptionTryEitherMap(SortEntity, new UnknownError())
                .Map(RunSaving)
                .GetOrElse((Either<Error, ShoppingListEntity>) new UnknownError());
        }

        private Option<(ShoppingListEntity shoppingListEntity, ShoppingListModule.ShoppingList result)>
            AddItemToList(Option<ItemDataCreateDto> itemToAdd) =>
            itemToAdd
                .Bind(i => GetShoppingListWithChildrenById(i.ShoppingListId)
                    .Map(dbList => (i, dbList)))
                .Map(pair =>
                {
                    var (itemDataCreateDto, shoppingListEntity) = pair;
                    var result = addItem(shoppingListEntity, (ItemDataEntity) itemDataCreateDto);
                    return (shoppingListEntity, result);
                });

        private Try<Either<Error, ShoppingListEntity>> SortTryEither(
            Try<Either<Error, ShoppingListEntity>> entityToSort)
            => entityToSort.Map(j =>
                j.Bind(k => SortEntity(k)
                    .Map(m => (Either<Error, ShoppingListEntity>) Right(m))
                    .GetOrElse(() => Left((Error) new UnknownError()))));

        public Either<Error, ShoppingListReadDto> ModifyShoppingList(
            Option<ItemDataActionDtoNoPassword> itemDataAction) =>
            itemDataAction
                .Pipe(ApplyItemDataAction)
                .Map(r =>
                    FSharpChoiceToEither(r.result)
                        .Map(i => (r.entityFromDb, i)))
                .OptionEitherMap(WrapSaving)
                .OptionEitherMap(SortTryEither)
                .OptionEitherMap(RunSaving)
                .Pipe(HandleSavingResultTypedGeneric)
                .Map(i => (ShoppingListReadDto) i);

        private Option<(ShoppingListEntity entityFromDb,
                FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors> result)>
            ApplyItemDataAction(Option<ItemDataActionDtoNoPassword> itemDataAction)
            => itemDataAction.Bind(i => GetShoppingListWithChildrenById(i.ShoppingListId)
                    .Map(j => (i, j).ToTuple()))
                .Bind(bothNonEmpty =>
                {
                    var (updateDto, entityFromDb) = bothNonEmpty;
                    return ItemDataActionDtoNoPassword.Actions.TryGetValue(
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

        private Try<Either<Error, ShoppingListEntity>> WrapSaving(
            (ShoppingListEntity, ShoppingListModule.ShoppingList) values)
        {
            var (entityFromDb, result) = values;
            _context.Entry(entityFromDb).CurrentValues.SetValues(result);
            _context.ItemDataEntities.RemoveRange(entityFromDb.ItemDataEntities);
            result.Items.ToList()
                .ForEach(i => entityFromDb.ItemDataEntities.Add(i));
            return Try(SaveChanges)
                .Map
                    <bool, Either<Error, ShoppingListEntity>>
                    (isSuccessful => isSuccessful
                        ? Right(entityFromDb)
                        : Left((Error) new SavingFailed()));
        }

        private Either<Error, ShoppingListEntity> RunSaving(Try<Either<Error, ShoppingListEntity>> trySave) =>
            trySave
                .Run().Match(
                    l => (Either<Error, ShoppingListEntity>) Left<Error>(new UnknownError()),
                    r => r);

        private static Either<Error, T> HandleSavingResultTypedGeneric<T>(
            Option<Either<ShoppingListErrors.ShoppingListErrors,
                Either<Error, T>>> result) =>
            result.Pipe(FlattenEitherErrors)
                .GetOrElse(Left((Error) new UnknownError()));

        private static Option<Either<Error, T>> FlattenEitherErrors<T>(
            Option<Either<ShoppingListErrors.ShoppingListErrors, Either<Error, T>>> option) =>
            OptionExt.Map(option, i =>
                FlattenErrors(i, l1 => new OtherError(l1)));

        private static Either<TL2, T> FlattenErrors<TL1, TL2, T>(
            Either<TL1, Either<TL2, T>> either, Func<TL1, TL2> errorConversion) => either.Match(
            l => Left(errorConversion(l)),
            r => r);

        public bool SaveChanges() => _context.SaveChanges() >= 0;

        private Option<ShoppingListEntity> GetShoppingListWithChildrenById(int id) =>
            (Option<ShoppingListEntity>) _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id)!;
    }
}