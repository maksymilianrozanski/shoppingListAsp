using ShoppingData;

namespace ShoppingList.Data
{
    public class SqlShoppingListRepo : IShoppingListRepo
    {

        private ShoppingListDbContext _context;

        public SqlShoppingListRepo(ShoppingListDbContext context)
        {
            _context = context;
        }

        public void CreateShoppingList(ShoppingListModule.ShoppingList shoppingList) => throw new System.NotImplementedException();
    }
}