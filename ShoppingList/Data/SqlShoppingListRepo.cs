using System;
using LaYumba.Functional;
using ShoppingList.Dtos;
using ShoppingList.Entities;
using static LaYumba.Functional.F;
using LaYumba.Functional.Option;
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
            ((Option<ShoppingListEntity>) _context.ShoppingListEntities.Find(id))
            .Map(i => (ShoppingListReadDto) i);

        public bool SaveChanges() => _context.SaveChanges() >= 0;
    }
}