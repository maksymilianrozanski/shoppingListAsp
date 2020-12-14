using System;
using System.Linq;
using LaYumba.Functional;
using Microsoft.EntityFrameworkCore;
using Microsoft.FSharp.Core;
using ShoppingData;
using ShoppingList.Dtos;
using ShoppingList.Entities;
using static LaYumba.Functional.F;
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

        private Option<ShoppingListEntity> GetShoppingListWithChildrenById(int id) =>
            _context.ShoppingListEntities
                .Include(i => i.ItemDataEntities)
                .FirstOrDefault(i => i.Id == id);

        public Option<ShoppingListReadDto> UpdateShoppingListEntity(Option<ShoppingListUpdateDto> updated) =>
            updated.Bind(i => GetShoppingListWithChildrenById(i.Id).Map(j => (i, j)))
                .Bind(bothNonEmpty =>
                    {
                        var (updateDto, entityFromDb) = bothNonEmpty;
                        var itemToSave = _context.Update(entityFromDb).Entity.Merge(updateDto);
                        return Try(SaveChanges)
                            .Map(result => result
                                ? Some((ShoppingListReadDto) itemToSave)
                                : null)
                            .Run()
                            .Match(exception =>
                            {
                                Console.WriteLine($"Exception during saving: ${exception}");
                                return null;
                            }, x => x);
                    }
                );

        public Option<ShoppingListReadDto> AddItemToShoppingList(Option<ItemDataCreateDto> itemToAdd)
        {
            Console.WriteLine("received itemToAdd");
            return itemToAdd.Bind(i => GetShoppingListWithChildrenById(i.ShoppingListId).Map(dbList => (i, dbList)))
                .Bind<(ItemDataCreateDto, ShoppingListEntity), ShoppingListReadDto>(
                    ((ItemDataCreateDto i, ShoppingListEntity dbList) pair) =>
                    {
                        var (itemDataCreateDto, shoppingListEntity) = pair;

                        var result =
                            ShoppingListModule.addItemIfPassword.Invoke((ItemDataEntity) itemDataCreateDto)
                                .Invoke(shoppingListEntity).Invoke(
                                    itemDataCreateDto.Password);

                        switch (result)
                        {
                            case FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors>.
                                Choice1Of2 list:

                                var withAddedItem = list.Item;

                                _context.Entry(shoppingListEntity).CurrentValues.SetValues(withAddedItem);
                                _context.ItemDataEntities.RemoveRange(shoppingListEntity.ItemDataEntities);
                                withAddedItem.Items.ToList().ForEach(i => shoppingListEntity.ItemDataEntities.Add(i));

                                return Try(SaveChanges)
                                    .Map(r => r
                                        ? Some(shoppingListEntity)
                                        : null)
                                    .Map(
                                        i =>
                                            i.Bind<ShoppingListEntity, ShoppingListReadDto>(
                                                j => (ShoppingListReadDto) j))
                                    .Run()
                                    .Match(exception =>
                                    {
                                        Console.WriteLine($"Exception during saving: ${exception}");
                                        return null;
                                    }, x =>
                                    {
                                        Console.WriteLine("Success");
                                        return x;
                                    });
                            case FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors>.
                                Choice2Of2 error:
                                Console.WriteLine("error during adding new item: " + ErrorTextValue(error.Item));
                                return None;
                            default:
                                throw new MatchFailureException();
                        }
                    });
        }

        public Option<ShoppingListReadDto> ModifyShoppingListItem(Option<ItemDataActionDto> itemDataAction)
            =>
                itemDataAction.Bind(i => GetShoppingListWithChildrenById(i.ShoppingListId)
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
                    .Bind
                    (r =>
                        {
                            switch (r.result)
                            {
                                case ShoppingListUpdatedChoice1Of2 success:
                                    _context.Entry(r.entityFromDb).CurrentValues.SetValues(success.Item);
                                    _context.ItemDataEntities.RemoveRange(r.entityFromDb.ItemDataEntities);
                                    success.Item.Items.ToList()
                                        .ForEach(i => r.entityFromDb.ItemDataEntities.Add(i));
                                    return Try(SaveChanges)
                                        .Map(isSuccessful => isSuccessful
                                            ? Some(r.entityFromDb)
                                            : null)
                                        .Map(i =>
                                            i.Bind<ShoppingListEntity, ShoppingListReadDto>(j =>
                                                (ShoppingListReadDto) j)
                                        ).Run()
                                        .Match(exception =>
                                        {
                                            Console.WriteLine($"Exception during saving: ${exception}");
                                            return null;
                                        }, x =>
                                        {
                                            Console.WriteLine("Success");
                                            return x;
                                        });
                                case FSharpChoice<ShoppingListModule.ShoppingList,
                                        ShoppingListErrors.ShoppingListErrors>.
                                    Choice2Of2 error:
                                    Console.WriteLine("error during adding new item: " +
                                                      ErrorTextValue(error.Item));
                                    return None;
                                default:
                                    throw new MatchFailureException();
                            }
                        }
                    );

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