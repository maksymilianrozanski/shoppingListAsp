using System.Collections.Generic;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using SharedTypes.Dtos;
using static LaYumba.Functional.F;
using static Waypoints.WaypointsModule;

namespace ShoppingList.Data.Waypoints
{
    public class WaypointsRepoHardcoded : IWaypointsRepo
    {
        public static readonly ShopWaypointsReadDto HardcodedWaypoints = new("big-market",
            new Waypoint("start", 699, 673),
            new Waypoint("checkout", 469, 584),
            new List<Waypoint>
            {
                new("DRINK_JUICE", 283, 295),
                new("VEGETABLES", 607, 332),
                new("CHEMISTRY", 369, 461),
                new("MEAT", 466, 164),
                new("VODKA_ALCOHOL", 292, 458),
                new("CANNED_PATE", 505, 466),
                new("GENERAL", 680, 298),
                new("KETCH_CONCETRATE_MUSTARD_MAJO_HORSERADISH", 506, 458),
                new("COFFEE TEA", 606, 464),
                new("COOKIES_BULK", 632, 457),
                new("GENERAL_FOOD", 544, 456),
                new("CHIPS_FLAKES", 579, 462),
                new("SWEETS", 629, 466),
                new("DAIRY_CHESSE", 469, 263),
                new("PHONES_ADJUSTMENTS", 467, 567),
                new("ICE_CREAMS_FROZEN", 546, 300),
                new("SPICES", 506, 483),
                new("SOCKS_THIGHTS", 783, 609),
                new("OILS", 403, 296),
                new("GROATS_RICE_PASTA", 550, 462),
                new("TABLETS", 454, 466),
                new("CIGARETTES", 252, 590),
                new("ART._HYGIENIC", 459, 466),
                new("BEER", 325, 593),
                new("CHEWING_GUM_LOLIPOPS", 474, 566),
                new("BREAD", 581, 460),
                new("FISH", 611, 263),
                new("WINE_ALCOHOL 18%", 240, 469),
                new("OCCASIONAL", 770, 515),
                new("PET'S FOOD", 368, 462),
                new("DISHES_FOR_CHILDREN", 502, 460),
                new("LUNCH DINING DISHES", 508, 455),
                new("ARTICLE_OF_HOUSEHOLD", 666, 470),
                new("PACKAGES", 708, 463),
                new("EGGS", 378, 169),
                new("MILK", 339, 300),
            }
        );

        public Option<ShopWaypointsReadDto> GetShopWaypoints(string shopName) =>
            shopName == "big-market" ? Some(HardcodedWaypoints) : new Option<ShopWaypointsReadDto>();

        public Option<int> GetShopWaypointsId(string shopName) =>
            shopName == "big-market" ? Some(1) : new None();
    }
}