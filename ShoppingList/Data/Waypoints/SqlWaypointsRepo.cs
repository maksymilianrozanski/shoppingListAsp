using FSharpPlus;
using LaYumba.Functional;
using SharedTypes.Entities;

namespace ShoppingList.Data.Waypoints
{
    public class SqlWaypointsRepo : IWaypointsRepo
    {
        private readonly ShoppingListDbContext _context;

        public SqlWaypointsRepo(ShoppingListDbContext context)
        {
            _context = context;
        }

        public Option<ShopWaypointsEntity> GetShopWaypointsEntity(string shopName) => _context.ShopWaypointsEntities
            .Find(i => i.Name == shopName);
    }
}