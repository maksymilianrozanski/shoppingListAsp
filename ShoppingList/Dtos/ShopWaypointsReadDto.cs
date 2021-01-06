using System.Collections.Generic;
using static Waypoints.WaypointsModule;

namespace ShoppingList.Dtos
{
    public class ShopWaypointsReadDto
    {
        public string Name { get; set; }

        public Waypoint Start { get; set; }
        public Waypoint Checkout { get; set; }
        public List<Waypoint> Waypoints { get; set; }

        public ShopWaypointsReadDto(string name, Waypoint start, Waypoint checkout, List<Waypoint> waypoints)
        {
            Name = name;
            Start = start;
            Checkout = checkout;
            Waypoints = waypoints;
        }
    }
}