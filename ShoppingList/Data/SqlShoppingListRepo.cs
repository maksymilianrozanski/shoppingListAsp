using System;
using ShoppingList.Dtos;
using ShoppingList.Entities;

namespace ShoppingList.Data
{
    public class SqlShoppingListRepo : IShoppingListRepo
    {
        private ShoppingListDbContext _context;

        public SqlShoppingListRepo(ShoppingListDbContext context)
        {
            _context = context;
        }

        public ShoppingListEntity CreateShoppingList(ShoppingListCreateDto shoppingList)
        {
            if (shoppingList == null)
                throw new ArgumentNullException(nameof(shoppingList));
            else
            {
                ShoppingListEntity toSave = shoppingList;
                _context.ShoppingListEntities.Add(toSave);
                return toSave;
            }
        }

        public ShoppingListEntity GetShoppingListEntityById(int id) =>
            _context.ShoppingListEntities.Find(id);

        public bool SaveChanges() => _context.SaveChanges() >= 0;
    }
}