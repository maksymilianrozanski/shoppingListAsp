using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using ShoppingList.Dtos;
using ShoppingList.Entities;
using static LaYumba.Functional.F;
using LaYumba.Functional.Option;
using Microsoft.EntityFrameworkCore;
using static LaYumba.Functional.OptionExt;
using None = LaYumba.Functional.Option.None;

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
                        return SaveChanges() ? Some((ShoppingListReadDto) itemToSave) : null;
                    }
                );

        public bool SaveChanges() => _context.SaveChanges() >= 0;
    }
}