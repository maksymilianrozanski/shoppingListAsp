using LaYumba.Functional;
using SharedTypes.Dtos;

namespace ShoppingList.Data.Waypoints
{
    public class SqlWaypointsRepo : IWaypointsRepo
    {
        private readonly ShoppingListDbContext _context;

        public SqlWaypointsRepo(ShoppingListDbContext context)
        {
            _context = context;
        }

        public Option<ShopWaypointsReadDto> GetShopWaypoints(string shopName) =>
            shopName == "big-market"
                ? WaypointsRepoHardcoded.HardcodedWaypoints
                : _context.ShopWaypointsEntities
                    .Find(i => i.Name == shopName)
                    .Bind(ShopWaypointsReadDto.ToOptionReadDto);
    }
}