using LaYumba.Functional;
using SharedTypes.Dtos;
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

        public Option<ShopWaypointsReadDto> GetShopWaypoints(string shopName) =>
            shopName == "big-market"
                ? WaypointsRepoHardcoded.HardcodedWaypoints
                : GetShopWaypointsEntity(shopName)
                    .Bind(ShopWaypointsReadDto.ToOptionReadDto);
        
        private Option<ShopWaypointsEntity> GetShopWaypointsEntity(string shopName) => _context.ShopWaypointsEntities
            .Find(i => i.Name == shopName);

        public Option<int> GetShopWaypointsId(string shopName) =>
            GetShopWaypointsEntity(shopName).Map(i => i.Id);
    }
}