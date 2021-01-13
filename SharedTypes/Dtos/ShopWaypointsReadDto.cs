using System.Collections.Generic;
using System.Text.Json;
using LaYumba.Functional;
using SharedTypes.Entities;
using static Waypoints.WaypointsModule;

namespace SharedTypes.Dtos
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

        public static Option<ShopWaypointsReadDto> Deserialize(string shopWaypointsJson)
            => JsonSerializer.Deserialize<ShopWaypointsReadDto>(shopWaypointsJson)
               ?? new Option<ShopWaypointsReadDto>();

        public static Option<ShopWaypointsReadDto> ToOptionReadDto(ShopWaypointsEntity entity) =>
            Deserialize(entity.ShopWaypointsReadDtoJson);
    }
}