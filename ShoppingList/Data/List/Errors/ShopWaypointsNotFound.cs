using LaYumba.Functional;

namespace ShoppingList.Data.List.Errors
{
    public sealed class ShopWaypointsNotFound : Error
    {
        public override string Message { get; } = "shop waypoints not found";
    }
}