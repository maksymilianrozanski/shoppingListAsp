using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;
using SharedTypes.Dtos;
using SharedTypes.Entities;
using ShoppingData;
using ShoppingList.Auth;
using ShoppingList.Data.List.Errors;
using ShoppingList.Utils;
using ShoppingListSorting;
using static ShoppingData.ShoppingListErrors;
using static ShoppingList.Auth.IdBasedAuthenticationHandler;
using ShoppingListErrors = ShoppingData.ShoppingListErrors;
using static LaYumba.Functional.F;

namespace ShoppingList.Data.List
{
    public partial class SqlShoppingListRepo
    {
        public Either<ShoppingListErrors.ShoppingListErrors, bool> UserExists(Option<string> queryLogin) =>
            queryLogin.Map(login =>
                    F.Try(() => _context.UserEntities.Any(i => i.Login == login)).Run()
                        .Match(l =>
                                F.Left(
                                    ShoppingListErrors.ShoppingListErrors.NewOtherError(l.Message)),
                            r =>
                                (Either<ShoppingListErrors.ShoppingListErrors, bool>) F.Right(r)))
                .GetOrElse(ShoppingListErrors.ShoppingListErrors.NewOtherError("empty input"));

        public Either<ShoppingListErrors.ShoppingListErrors, UserReadDto> GetUserById(Option<int> userId) =>
            userId.Map(FindUserById).GetOrElse(ShoppingListErrors.ShoppingListErrors.NotFound);

        public Either<ShoppingListErrors.ShoppingListErrors, UserReadDto> FindUserById(int userId) =>
            FindUser(u => u.Id == userId);

        public Either<ShoppingListErrors.ShoppingListErrors, UserReadDto> FindUserByLogin(string login) =>
            FindUser(u => u.Login == login);

        public Either<ShoppingListErrors.ShoppingListErrors, UserReadDto> FindUser(Func<UserEntity, bool> predicate) =>
            F.Try(() => _context.UserEntities.Find(predicate.Invoke)
                    .Map(i => (UserReadDto) i))
                .Map(i => i
                    .Map(j => (Either<ShoppingListErrors.ShoppingListErrors, UserReadDto>) F.Right(j)))
                .Run()
                .Match(l => F.Left(ShoppingListErrors.ShoppingListErrors.NewOtherError(l.Message)),
                    r =>
                        r.GetOrElse(F.Left(ShoppingListErrors.ShoppingListErrors.NotFound)));

        private Option<ShoppingListEntity> GetShoppingListEntityById(int id) =>
            _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id)
                .Pipe(i => (Option<ShoppingListEntity>) i!);

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

        public Either<ShoppingListErrors.ShoppingListErrors, IEnumerable<ShoppingListReadDto>> GetUserShoppingLists(
            string userLogin) =>
            F.Try(() => _context.UserEntities.Include(i => i.ShoppingListEntities)
                    .First(i => i.Login == userLogin).ShoppingListEntities
                    .Map(i => (ShoppingListReadDto) i))
                .Run()
                .Match(l =>
                        F.Left(ShoppingListErrors.ShoppingListErrors.NewOtherError(l.Message)),
                    r =>
                        (Either<ShoppingListErrors.ShoppingListErrors, IEnumerable<ShoppingListReadDto>>) F.Right(r)
                );

        private Option<ShoppingListEntity> SortEntity(ShoppingListEntity entity) =>
            ShouldTryFindWaypoints(entity)
                .Map(k =>
                    MatchWaypointsToShoppingList(k)
                        .Map(SortToOptimalOrder)
                        .Map(i => (ShoppingListEntity) i)
                )
                .Match(() => entity, r =>
                    r.Match(_ => entity, right => right)
                );

        internal static Option<ShoppingListEntity>
            ShouldTryFindWaypoints(Option<ShoppingListEntity> shoppingListEntity) =>
            shoppingListEntity.Bind(i =>
                i.ShopWaypointsEntityId != null ? F.Some(i) : new None());

        internal Either<ShopWaypointsNotFound, (ShopWaypointsReadDto, ShoppingListModule.ShoppingList)>
            MatchWaypointsToShoppingList(
                ShoppingListEntity shoppingListEntity) =>
            (_context.ShoppingListEntities
                 .IgnoreAutoIncludes()
                 .Where(i => i.ShopWaypointsEntity != null &&
                             shoppingListEntity.ShopWaypointsEntityId == i.ShopWaypointsEntityId)
                 .Select(i => i.ShopWaypointsEntity)
                 .FirstOrDefault()
             ?? new Option<ShopWaypointsEntity>())
            .Bind(ShopWaypointsReadDto.ToOptionReadDto)
            .Map(i =>
                (Either<ShopWaypointsNotFound, (ShopWaypointsReadDto, ShoppingListModule.ShoppingList)>)
                (i, shoppingListEntity))
            .GetOrElse(() =>
                new ShopWaypointsNotFound());

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
                    if (i.Password == password) return F.Right(i.Id);
                    else return F.Left(ShoppingListErrors.ShoppingListErrors.IncorrectPassword);
                });

        public Either<ShoppingListErrors.ShoppingListErrors, UserReadDto>
            AuthenticateUser2(UserLoginData2 userLoginData) =>
            F.Try(() =>
                    _context.UserEntities.FirstOrDefault(i =>
                        i.Login == userLoginData.Username && i.Password == userLoginData.Password)
                ).Run()
                .Match(l => F.Left(ShoppingListErrors.ShoppingListErrors.NewOtherError(l.Message)),
                    r => r == null
                        ? F.Left(ShoppingListErrors.ShoppingListErrors.NotFound)
                        : (Either<ShoppingListErrors.ShoppingListErrors, UserReadDto>) F.Right((UserReadDto) r!));

        public Either<ShoppingListErrors.ShoppingListErrors, (User, int)> ValidateAccess
            ((User, int) userWithListId) =>
            GetShoppingList(userWithListId.Item2).Bind(list =>
            {
                if (list.UserId == userWithListId.Item1.UserId)
                {
                    return (Either<ShoppingListErrors.ShoppingListErrors, (User, int)>) Right(userWithListId);
                }
                else
                {
                    return Left(ShoppingListErrors.ShoppingListErrors.IncorrectUser);
                }
            });

        private Try<Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>> SortTryEither(
            Try<Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>> entityToSort)
            => entityToSort
                .Map(j =>
                    j.Bind
                    (k => SortEntity(k)
                        .Map(m => (Either<ShoppingListErrors.ShoppingListErrors, ShoppingListEntity>) F.Right(m))
                        .GetOrElse(
                            () => F.Left(ShoppingListErrors.ShoppingListErrors.NewOtherError(new UnknownError())))
                    ));

        private Option<ShoppingListEntity> GetShoppingListWithChildrenById(int id) =>
            (Option<ShoppingListEntity>) _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id)!;
    }
}