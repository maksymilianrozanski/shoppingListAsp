using System;
using ShoppingList.Dtos;

namespace ShoppingList.Data
{
    public class SqlShoppingListRepo : IShoppingListRepo
    {
        private ShoppingListDbContext _context;

        public SqlShoppingListRepo(ShoppingListDbContext context)
        {
            _context = context;
        }

        public void CreateShoppingList(ShoppingListCreateDto shoppingList)
        {
            if (shoppingList == null)
                throw new ArgumentNullException(nameof(shoppingList));
            else
                _context.ShoppingListEntities.Add(shoppingList);
        }

        public bool SaveChanges() => _context.SaveChanges() >= 0;
    }
}