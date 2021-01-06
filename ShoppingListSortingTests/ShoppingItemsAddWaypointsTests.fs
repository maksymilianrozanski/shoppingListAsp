module ShoppingListSortingTests.ShoppingItemsAddWaypointsTests


open FSharpPlus
open NUnit.Framework
open NUnit.Framework.Internal
open ShoppingData
open ShoppingList.Dtos
open ShoppingListSorting
open ShoppingItemModule
open ShoppingListModule
open ShoppingItemsPredicting
open System.Collections.Generic
open ShoppingListSorting.ShoppingItemsAddWaypoints
open Waypoints.WaypointsModule

[<SetUp>]
let Setup () = ()

[<Test>]
let Test1 () = Assert.Pass()

[<Test>]
let ``should pick waypoint with matching name`` () =
    let waypoints =
        ShopWaypointsReadDto
            ("big-market",
             { name = "start"; x = 0L; y = 0L },
             { name = "checkout"
               x = 100L
               y = 100L },
             [ { name = "JUICES"; x = 10L; y = 10L }
               { name = "BREAD"; x = 20L; y = 20L } ]
             |> ResizeArray)

    let expected =
        Some({ name = "BREAD"; x = 20L; y = 20L })

    let itemData =
        ({ Id = 0
           Name = "brown bread"
           Quantity = 1
           ItemType = ItemType.ToBuy },
         PredictedShopsDepartment("BREAD"))

    let result = pickSingleWaypoint itemData waypoints

    Assert.AreEqual(expected, result)
