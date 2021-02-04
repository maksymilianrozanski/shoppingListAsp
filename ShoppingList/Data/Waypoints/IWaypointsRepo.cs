using LaYumba.Functional;
using SharedTypes.Dtos;

namespace ShoppingList.Data.Waypoints
{
    public interface IWaypointsRepo
    {
        Option<ShopWaypointsReadDto> GetShopWaypoints(string shopName);

        Option<int> GetShopWaypointsId(string shopName);
    }
}