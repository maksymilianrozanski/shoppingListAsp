module ShoppingListSortingTests.ShoppingItemsAddWaypointsTests


open FSharpPlus
open NUnit.Framework
open NUnit.Framework
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

    let result = pickSingleWaypoint waypoints itemData

    Assert.AreEqual(expected, result)

[<Test>]
let ``should pick waypoints for all items`` () =
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

    let shoppingList: ShoppingListWithDepartment =
        { Id = 0
          Name = "My-shopping-list"
          Password = "pass"
          ShopName = "todo"
          Items =
              ResizeArray<ItemDataWithPredictedType>
                  [ ({ Id = 0
                       Name = "Orange juice"
                       Quantity = 1
                       ItemType = ItemType.ToBuy },
                     PredictedShopsDepartment("JUICES"))
                    ({ Id = 1
                       Name = "Carrot"
                       Quantity = 2
                       ItemType = ItemType.ToBuy },
                     PredictedShopsDepartment("VEGETABLES"))
                    ({ Id = 2
                       Name = "Cheddar cheese"
                       Quantity = 1
                       ItemType = ItemType.ToBuy },
                     PredictedShopsDepartment("CHEESE")) ] }

    let expected: ShoppingListWithWaypoints =
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

    let result =
        addWaypointsToShoppingList waypoints shoppingList

    Assert.AreEqual(expected, result)
