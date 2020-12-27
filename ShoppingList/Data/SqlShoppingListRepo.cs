using System;
using System.Linq;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;
using ShoppingData;
using ShoppingList.Dtos;
using ShoppingList.Dtos.Protected;
using ShoppingList.Entities;
using ShoppingList.Utils;
using static LaYumba.Functional.F;
using static ShoppingList.Data.IShoppingListRepo;
using static ShoppingList.Data.IShoppingListRepo.RepoRequestError;
using ActionFoundResult =
    System.Tuple<ShoppingList.Dtos.ItemDataActionDto, ShoppingList.Entities.ShoppingListEntity, Microsoft.FSharp.Core.
        FSharpFunc<string, Microsoft.FSharp.Core.FSharpFunc<int, Microsoft.FSharp.Core.FSharpFunc<
            ShoppingData.ShoppingListModule.ShoppingList, Microsoft.FSharp.Core.FSharpFunc<string, Microsoft.FSharp.Core
                .FSharpChoice<ShoppingData.ShoppingListModule.ShoppingList,
                    ShoppingData.ShoppingListErrors.ShoppingListErrors>>>>>
    >;
using ShoppingListUpdatedChoice1Of2 =
    Microsoft.FSharp.Core.FSharpChoice<ShoppingData.ShoppingListModule.ShoppingList,
        ShoppingData.ShoppingListErrors.ShoppingListErrors>.
    Choice1Of2;

namespace ShoppingList.Data
{
    public class SqlShoppingListRepo : IShoppingListRepo
    {
        private ShoppingListDbContext _context;

        public SqlShoppingListRepo(ShoppingListDbContext context)
        {
            _context = context;
        }

        public Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList) =>
            shoppingList.Map(i => _context.ShoppingListEntities.Add(i).Entity)
                .Bind(i => SaveChanges() ? Some(i) : null)
                .Map(i => (ShoppingListReadDto) i);

        public Option<ShoppingListReadDto> GetShoppingListEntityById(int id) =>
            ((Option<ShoppingListEntity>) _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id))
            .Map(i => (ShoppingListReadDto) i);

        public Either<RepoRequestError, ShoppingListReadDto> GetShoppingListEntityByIdIfPassword(
            Option<ShoppingListGetRequest> request) =>
            request
                .Bind(r =>
                    _context.ShoppingListEntities.Find(i => i.Id == r.Id)
                        .Map(list => (r, list))
                        .Map(VerifyPassword))
                .Map(maybeVerified =>
                    maybeVerified.Map(GetShoppingListEntityById))
                .Map(MapToNotFoundIfEmpty)
                .GetOrElse(NotFound);

        public Either<ShoppingListErrors.ShoppingListErrors, int> PasswordMatchesShoppingList(int shoppingListId,
            string password) =>
            GetShoppingListEntityById(shoppingListId)
                .Map<ShoppingListReadDto, Either<ShoppingListErrors.ShoppingListErrors, int>>(i =>
                {
                    if (i.Password == password) return Right(i.Id);
                    else return Left(ShoppingListErrors.ShoppingListErrors.IncorrectPassword);
                })
                .GetOrElse(Left(ShoppingListErrors.ShoppingListErrors.ListItemNotFound));

        private static Either<RepoRequestError, T> MapToNotFoundIfEmpty<T>(Either<RepoRequestError, Option<T>> input) =>
            input.Bind(i => i.Map(j => (Either<RepoRequestError, T>) Right(j))
                .GetOrElse(Left(NotFound)));

        private Either<RepoRequestError, int> VerifyPassword(
            (ShoppingListGetRequest, ShoppingListEntity) tuple) =>
            tuple.Item1.Password == tuple.Item2.Password
                ? Right(tuple.Item2.Id)
                : Left(IncorrectPassword);

        private Option<ShoppingListEntity> GetShoppingListWithChildrenById(int id) =>
            _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id);

        public Either<string, ShoppingListReadDto> AddItemToShoppingList(Option<ItemDataCreateDto> itemToAdd)
        {
            Console.WriteLine("received AddItemToShoppingListNoPassword request");
            return itemToAdd
                .Bind(i => GetShoppingListWithChildrenById(i.ShoppingListId)
                    .Map(dbList => (i, dbList)))
                .Map(pair =>
                {
                    var (itemDataCreateDto, shoppingListEntity) = pair;
                    var result =
                        ShoppingListModule.addItemIfPassword
                            .Invoke((ItemDataEntity) itemDataCreateDto)
                            .Invoke(shoppingListEntity)
                            .Invoke(itemDataCreateDto.Password);
                    return (shoppingListEntity, result);
                })
                .Map(r =>
                    EitherUtils.FSharpChoiceToEither(r.result)
                        .Map(i => (r.shoppingListEntity, i).ToTuple())
                        .Map(TryToSaveShoppingList))
                .Map(i =>
                    i.Match(err => Left(ErrorTextValue(err)),
                        either =>
                            either
                    )).GetOrElse(Left("unknown error"));
        }

        public Either<string, ShoppingListReadDto> AddItemToShoppingListNoPassword(
            Option<ItemDataCreateDtoNoPassword> itemToAdd)
        {
            Console.WriteLine("received AddItemToShoppingListNoPassword (without password)request");
            return itemToAdd
                .Bind(i => GetShoppingListWithChildrenById(i.ShoppingListId)
                    .Map(dbList => (i, dbList)))
                .Map(pair =>
                {
                    var (itemDataCreateDto, shoppingListEntity) = pair;
                    var result = ShoppingListModule.addItem(shoppingListEntity, (ItemDataEntity) itemDataCreateDto);
                    return (shoppingListEntity, result);
                })
                .Map(i => (i.shoppingListEntity, i.result).ToTuple())
                .Map(TryToSaveShoppingList)
                .Map(i => i
                    .Match<Either<string, ShoppingListReadDto>>(err => Left(err),
                        either =>
                            either
                    )).GetOrElse(Left("unknown error"));
        }

        public Either<string, ShoppingListReadDto> ModifyShoppingListItem(Option<ItemDataActionDto> itemDataAction)
        {
            Console.WriteLine("received ModifyShoppingListItem request");
            return itemDataAction.Bind(i => GetShoppingListWithChildrenById(i.ShoppingListId)
                    .Map(j => (i, j).ToTuple()))
                .Bind(bothNonEmpty =>
                {
                    var (updateDto, entityFromDb) = bothNonEmpty;

                    if (ItemDataActionDto.Actions.TryGetValue(
                        (ItemDataActionDto.ItemDataActions) updateDto.ActionNumber, out var modifyingFunction))
                    {
                        var r = (updateDto, entityFromDb, modifyingFunction);
                        return Some(r.ToTuple());
                    }
                    else
                    {
                        return null;
                    }
                })
                .Map(r =>
                {
                    var (updateDto, entityFromDb, modifyingFunction) = r;
                    var result = modifyingFunction
                        .Invoke(updateDto.User)
                        .Invoke(updateDto.ItemId)
                        .Invoke(entityFromDb)
                        .Invoke(updateDto.Password);
                    return (entityFromDb, result);
                })
                .Map(r =>
                    EitherUtils.FSharpChoiceToEither(r.result)
                        .Map(i => (r.entityFromDb, i).ToTuple())
                        .Map(TryToSaveShoppingList))
                .Map(i =>
                    i.Match(err => Left(ErrorTextValue(err)),
                        either =>
                            either
                    )).GetOrElse(Left("unknown error"));
        }

        public Either<string, ShoppingListReadDto> ModifyShoppingListItemNoPassword(
            Option<ItemDataActionDtoNoPassword> itemDataAction)
        {
            return itemDataAction.Bind(i => GetShoppingListWithChildrenById(i.ShoppingListId)
                    .Map(j => (i, j).ToTuple()))
                .Bind(bothNonEmpty =>
                {
                    var (updateDto, entityFromDb) = bothNonEmpty;
                    if (ItemDataActionDtoNoPassword.Actions.TryGetValue(
                        (ItemDataActionDto.ItemDataActions) updateDto.ActionNumber, out var modifyingFunction))
                    {
                        var r = (updateDto, entityFromDb, modifyingFunction);
                        return Some(r.ToTuple());
                    }
                    else
                    {
                        return null;
                    }
                })
                .Map(r =>
                {
                    var (updateDto, entityFromDb, modifyingFunction) = r;
                    var result = modifyingFunction
                        .Invoke(updateDto.User)
                        .Invoke(updateDto.ItemId)
                        .Invoke((entityFromDb));
                    return (entityFromDb, result);
                })
                //todo: remove duplicated code
                .Map(r =>
                    EitherUtils.FSharpChoiceToEither(r.result)
                        .Map(i => (r.entityFromDb, i).ToTuple())
                        .Map(TryToSaveShoppingList))
                .Map(i =>
                    i.Match(err => Left(ErrorTextValue(err)),
                        either =>
                            either
                    )).GetOrElse(Left("unknown error"));
        }

        private Either<string, ShoppingListReadDto> TryToSaveShoppingList(
            Tuple<ShoppingListEntity, ShoppingListModule.ShoppingList> values)
        {
            var (entityFromDb, result) = values;
            _context.Entry(entityFromDb).CurrentValues.SetValues(result);
            _context.ItemDataEntities.RemoveRange(entityFromDb.ItemDataEntities);
            result.Items.ToList()
                .ForEach(i => entityFromDb.ItemDataEntities.Add(i));
            return Try(SaveChanges)
                .Map
                    <bool, Either<string, ShoppingListReadDto>>
                    (isSuccessful => isSuccessful
                        ? Right((ShoppingListReadDto) entityFromDb)
                        : Left("saving failed"))
                .Run()
                .Match(exception =>
                {
                    Console.WriteLine($"Exception during saving: ${exception}");
                    return Left($"Exception during saving: ${exception}");
                }, x =>
                {
                    Console.WriteLine("Success");
                    return x;
                });
        }

        private static string ErrorTextValue(ShoppingListErrors.ShoppingListErrors error)
        {
            return error switch
            {
                var x when x.IsForbiddenOperation => nameof(ShoppingListErrors.ShoppingListErrors.ForbiddenOperation),
                var x when x.IsIncorrectPassword => nameof(ShoppingListErrors.ShoppingListErrors.IncorrectPassword),
                var x when x.IsIncorrectUser => nameof(ShoppingListErrors.ShoppingListErrors.IncorrectUser),
                var x when x.IsListItemNotFound => nameof(ShoppingListErrors.ShoppingListErrors.ListItemNotFound),
                var x when x.IsItemWithIdAlreadyExists => nameof(ShoppingListErrors.ShoppingListErrors
                    .ItemWithIdAlreadyExists),
                _ => throw new MatchFailureException()
            };
        }

        public bool SaveChanges() => _context.SaveChanges() >= 0;
    }
}