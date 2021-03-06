using System.Collections.Generic;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using static LaYumba.Functional.F;
using NUnit.Framework;
using SharedTypes.Dtos;
using Waypoints;

namespace SharedTypesTests
{
    public class ShopWaypointsReadDtoTests
    {
        [Test]
        public void ShouldDeserializeJson()
        {
            const string input = @"{
  ""Name"": ""small-shop"",
  ""Start"": {
    ""name"": ""start"",
    ""x"": 10,
    ""y"": 10
  },
  ""Checkout"": {
    ""name"": ""checkout"",
    ""x"": 100,
    ""y"": 100
  },
  ""Waypoints"": [
    {
      ""name"": ""vegetables"",
      ""x"": 20,
      ""y"": 20
    },
    {
      ""name"": ""fruits"",
      ""x"": 80,
      ""y"": 40
    }
  ]
}";

            var expected = new ShopWaypointsReadDto("small-shop",
                new WaypointsModule.Waypoint("start", 10, 10),
                new WaypointsModule.Waypoint("checkout", 100, 100),
                new List<WaypointsModule.Waypoint>
                {
                    new("vegetables", 20, 20),
                    new("fruits", 80, 40)
                }
            );

            var result = ShopWaypointsReadDto.Deserialize(input);

            result.Match(
                () => Assert.Fail("should have matched to some"),
                dto =>
                {
                    Assert.AreEqual(expected.Start, dto.Start);
                    Assert.AreEqual(expected.Checkout, dto.Checkout);
                    Assert.AreEqual(expected.Waypoints, dto.Waypoints);
                }
            );
        }
    }
}