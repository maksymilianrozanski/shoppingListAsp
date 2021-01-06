using LaYumba.Functional;
using LaYumba.Functional.Option;
using ShoppingList.Dtos;
using static LaYumba.Functional.F;

namespace ShoppingList.Data
{
    public class WaypointsRepoHardcoded : IWaypointsRepo
    {
        public Option<ShopWaypointsReadDto> GetShopWaypoints(string shopName) =>
            shopName == "big-market" ? Some(new ShopWaypointsReadDto()) : null!;
    }
}