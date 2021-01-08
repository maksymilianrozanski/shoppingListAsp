using LaYumba.Functional;
using SharedTypes.Dto;

namespace ShoppingList.Data
{
    public interface IWaypointsRepo
    {
        Option<ShopWaypointsReadDto> GetShopWaypoints(string shopName);
    }
}