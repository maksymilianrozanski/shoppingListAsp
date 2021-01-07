module ShoppingListSortingTests.WaypointsOrderTests

open NUnit.Framework
open NUnit.Framework
open ShoppingData.ShoppingItemModule
open ShoppingList.Dtos
open ShoppingListSorting.ShoppingItemsPredicting
open ShoppingListSorting
open ShoppingListSorting.ShoppingItemsAddWaypoints
open Waypoints.WaypointsModule
open WaypointsOrder

[<SetUp>]
let Setup () = ()

let waypoints =
    ShopWaypointsReadDto
        ("big-market",
         { name = "start"; x = 0L; y = 0L },
         { name = "checkout"
           x = 100L
           y = 100L },
         [ { name = "JUICES"; x = 10L; y = 10L }
           { name = "BREAD"; x = 20L; y = 20L }
           { name = "CHEESE"; x = 30L; y = 30L } ]
         |> ResizeArray)

let shoppingList: ShoppingListWithWaypoints =
    { Id = 0
      Name = "My-shopping-list"
      Password = "pass"
      ShopName = "todo"
      Items =
          [ ({ Id = 0
               Name = "Orange juice"
               Quantity = 1
               ItemType = ItemType.ToBuy },
             PredictedShopsDepartment("JUICES"),
             Some(waypoints.Waypoints.Item(0)))
            ({ Id = 1
               Name = "Carrot"
               Quantity = 2
               ItemType = ItemType.ToBuy },
             PredictedShopsDepartment("VEGETABLES"),
             None)
            ({ Id = 2
               Name = "Cheddar cheese"
               Quantity = 1
               ItemType = ItemType.ToBuy },
             PredictedShopsDepartment("CHEESE"),
             Some(waypoints.Waypoints.Item(2))) ]
      StartAndCheckout = (waypoints.Start, waypoints.Checkout) }

[<Test>]
let ``should return list of distinct waypoints`` () =
    let expected =
        [ waypoints.Waypoints.Item(0)
          waypoints.Waypoints.Item(2) ]

    let result = distinctAndDefinedWaypoints shoppingList

    Assert.AreEqual(expected, result)

[<Test>]
let ``should return list of waypoints sorted in expected order`` () =
    let expected = [ "JUICES"; "CHEESE" ]
    let result = sortWaypoints shoppingList
    Assert.AreEqual(expected, result)
