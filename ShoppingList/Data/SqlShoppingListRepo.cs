using System;
using System.Linq;
using LaYumba.Functional;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;
using ShoppingData;
using ShoppingList.Dtos;
using ShoppingList.Dtos.Protected;
using ShoppingList.Entities;
using ShoppingList.Utils;
using static LaYumba.Functional.F;
using ShoppingListUpdatedChoice1Of2 =
    Microsoft.FSharp.Core.FSharpChoice<ShoppingData.ShoppingListModule.ShoppingList,
        ShoppingData.ShoppingListErrors.ShoppingListErrors>.Choice1Of2;

namespace ShoppingList.Data
{
    public class SqlShoppingListRepo : IShoppingListRepo
    {
        private readonly ShoppingListDbContext _context;
        private readonly IWaypointsRepo _waypointsRepo;

        public SqlShoppingListRepo(ShoppingListDbContext context, IWaypointsRepo waypointsRepo)
        {
            _context = context;
            _waypointsRepo = waypointsRepo;
        }

        public Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList) =>
            shoppingList.Map(i => _context.ShoppingListEntities.Add(i).Entity)
                .Bind(i => SaveChanges() ? Some(i) : null!)
                .Map(i => (ShoppingListReadDto) i);

        public Option<ShoppingListReadDto> GetShoppingListEntityById(int id) =>
            _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id)
                .Pipe(i => (Option<ShoppingListEntity>) i!)
                .Map(i => (ShoppingListReadDto) i);

        public Either<ShoppingListErrors.ShoppingListErrors, int> PasswordMatchesShoppingList(int shoppingListId,
            string password) =>
            GetShoppingListEntityById(shoppingListId)
                .Map<ShoppingListReadDto, Either<ShoppingListErrors.ShoppingListErrors, int>>(i =>
                {
                    if (i.Password == password) return Right(i.Id);
                    else return Left(ShoppingListErrors.ShoppingListErrors.IncorrectPassword);
                })
                .GetOrElse(Left(ShoppingListErrors.ShoppingListErrors.ListItemNotFound));

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

        public Either<string, ShoppingListReadDto> ModifyShoppingListItemNoPassword(
            Option<ItemDataActionDtoNoPassword> itemDataAction) =>
            itemDataAction.Bind(i => GetShoppingListWithChildrenById(i.ShoppingListId)
                    .Map(j => (i, j).ToTuple()))
                .Bind(bothNonEmpty =>
                {
                    var (updateDto, entityFromDb) = bothNonEmpty;
                    return ItemDataActionDtoNoPassword.Actions.TryGetValue(
                        (ItemDataActionDto.ItemDataActions)
                        updateDto.ActionNumber, out var modifyingFunction)
                        ? Some((updateDto, entityFromDb, modifyingFunction).ToTuple())
                        : null!;
                })
                .Map(r =>
                {
                    var (updateDto, entityFromDb, modifyingFunction) = r;
                    var result = modifyingFunction
                        .Invoke(updateDto.User)
                        .Invoke(updateDto.ItemId)
                        .Invoke(entityFromDb);
                    return (entityFromDb, result);
                })
                .Map(r =>
                    EitherUtils.FSharpChoiceToEither(r.result)
                        .Map(i => (r.entityFromDb, i).ToTuple())
                        .Map(TryToSaveShoppingList))
                .Map(i =>
                    i.Match(error => Left(ErrorTextValue(error)),
                        either => either
                    )).GetOrElse(Left("unknown error"));

        public bool SaveChanges() => _context.SaveChanges() >= 0;

        private Option<ShoppingListEntity> GetShoppingListWithChildrenById(int id) =>
            (Option<ShoppingListEntity>) _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id)!;

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
                }, x => x);
        }

        private static string ErrorTextValue(ShoppingListErrors.ShoppingListErrors error) =>
            error switch
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
}