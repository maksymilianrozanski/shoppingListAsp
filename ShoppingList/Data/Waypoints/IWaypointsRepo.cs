using LaYumba.Functional;
using SharedTypes.Entities;

namespace ShoppingList.Data.Waypoints
{
    public interface IWaypointsRepo
    {
        public Option<ShopWaypointsEntity> GetShopWaypointsEntity(string shopName);
    }
}