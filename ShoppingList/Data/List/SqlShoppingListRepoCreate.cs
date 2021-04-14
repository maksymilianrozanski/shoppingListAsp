using System;
using System.Linq;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.EntityFrameworkCore;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using SharedTypes.Entities;
using ShoppingData;
using ShoppingList.Data.List.Errors;
using ShoppingList.Utils;
using static LaYumba.Functional.F;
using GroceryPredictionPool = Microsoft.Extensions.ML.PredictionEnginePool<GroceryClassification.GroceryData,
    GroceryClassification.GroceryItemPrediction>;

namespace ShoppingList.Data.List
{
    public partial class SqlShoppingListRepo
    {
        public Either<ShoppingListErrors.ShoppingListErrors, UserCreatedReadDto> CreateUser(
            Option<UserCreateDto> userCreateDto) =>
            CanCreateUser(userCreateDto)
                .Map(canCreate =>
                    Try(() =>
                            _context.UserEntities.Add(canCreate).Entity)
                        .Map(i => SaveChanges() ? Some((UserCreatedReadDto) i) : new None())
                        .Run()
                        .Match(l => F.Left(ShoppingListErrors.ShoppingListErrors.CreatingUserFailed), r =>
                            r.Map(i => (Either<ShoppingListErrors.ShoppingListErrors, UserCreatedReadDto>) Right(i))
                                .GetOrElse(
                                    (Either<ShoppingListErrors.ShoppingListErrors, UserCreatedReadDto>)
                                    F.Left(ShoppingListErrors.ShoppingListErrors.CreatingUserFailed))))
                .Bind(i => i);

        private Either<ShoppingListErrors.ShoppingListErrors, UserCreateDto> CanCreateUser(
            Option<UserCreateDto> userCreateDto) =>
            userCreateDto.Map(createDto => (createDto, UserExists(createDto.Login)))
                .Map(i => i.Item2.Match(l =>
                        (Either<ShoppingListErrors.ShoppingListErrors, UserCreateDto>)
                        Left(l),
                    exists => exists
                        ? F.Left(ShoppingListErrors.ShoppingListErrors.LoginAlreadyExists)
                        : Right(i.createDto)))
                .GetOrElse(F.Left(ShoppingListErrors.ShoppingListErrors.NewOtherError("empty input")));

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
                        var result = ShoppingListModule.addItem(shoppingListEntity, (ItemDataEntity) itemDataCreateDto);
                        return (shoppingListEntity, result);
                    });

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> CreateShoppingList(
            Option<ShoppingListCreateDto> shoppingList) =>
            Try(() =>
                    shoppingList
                        .Map(i => MatchWaypoints(i)
                            .Pipe(_ => _context.Database.CloseConnection())
                            .Map(AttachWaypointsEntity)))
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
                                readDto =>
                                    (Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>)
                                    Right(readDto)
                            ))
                        .GetOrElse(
                            (Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>)
                            ShoppingListErrors.ShoppingListErrors.NewOtherError(new SavingFailed()))
                );

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> CreateShoppingList2(
            Option<ShoppingListWithUserCreateDto> shoppingList) =>
            Try(() =>
                    shoppingList
                        .Map(i => MatchWaypoints(i)
                            .Pipe(_ => _context.Database.CloseConnection())
                            .Map(AttachWaypointsEntity)
                            .Map(listEntity =>
                            {
                                listEntity.UserEntityId = i.UserId;
                                return listEntity;
                            })
                        ))
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
                                readDto =>
                                    (Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>)
                                    Right(readDto)
                            ))
                        .GetOrElse(
                            (Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>)
                            ShoppingListErrors.ShoppingListErrors.NewOtherError(new SavingFailed()))
                );
    }
}