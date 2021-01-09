using LaYumba.Functional;
using SharedTypes.Dtos;

namespace ShoppingList.Data
{
    public interface IWaypointsRepo
    {
        Option<ShopWaypointsReadDto> GetShopWaypoints(string shopName);
    }
}