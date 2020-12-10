using LaYumba.Functional;
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

        public Option<ShoppingListEntity> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList) =>
            shoppingList.Map(i => _context.ShoppingListEntities.Add(i).Entity);

        public Option<ShoppingListEntity> GetShoppingListEntityById(int id) =>
            _context.ShoppingListEntities.Find(id);

        public bool SaveChanges() => _context.SaveChanges() >= 0;
    }
}