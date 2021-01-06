using LaYumba.Functional;
using ShoppingList.Dtos;

namespace ShoppingList.Data
{
    public interface IWaypointsRepo
    {
        Option<ShopWaypointsReadDto> GetShopWaypoints(string shopName);
    }
}