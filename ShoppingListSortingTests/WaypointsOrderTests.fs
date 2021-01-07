module ShoppingListSortingTests.WaypointsOrderTests

open NUnit.Framework
open NUnit.Framework
open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListModule
open ShoppingList.Dtos
open ShoppingListSorting.ShoppingItemsPredicting
open ShoppingListSorting
open ShoppingListSorting.ShoppingItemsAddWaypoints
open Waypoints.WaypointsModule
open WaypointsOrder

[<SetUp>]
let Setup () = ()

let waypoints1 =
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
             Some(waypoints1.Waypoints.Item(0)))
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
             Some(waypoints1.Waypoints.Item(2))) ]
      StartAndCheckout = (waypoints1.Start, waypoints1.Checkout) }

[<Test>]
let ``should return list of distinct waypoints`` () =
    let expected =
        [ waypoints1.Waypoints.Item(0)
          waypoints1.Waypoints.Item(2) ]

    let result = distinctAndDefinedWaypoints shoppingList

    Assert.AreEqual(expected, result)

[<Test>]
let ``should return list of waypoints sorted in expected order`` () =
    let shoppingListWithDepartment: ShoppingListWithDepartment =
        { Id = 0
          Name = "My-shopping-list"
          Password = "pass"
          ShopName = "todo"
          Items =
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
                 PredictedShopsDepartment("CHEESE")) ]
              |> ResizeArray }

    let waypoints2 =
        ShopWaypointsReadDto
            ("big-market",
             { name = "start"; x = 0L; y = 0L },
             { name = "checkout"
               x = 100L
               y = 100L },
             [ { name = "JUICES"; x = 30L; y = 30L }
               { name = "BREAD"; x = 20L; y = 20L }
               { name = "CHEESE"; x = 10L; y = 10L } ]
             |> ResizeArray)

    let expected1 = [ "JUICES"; "CHEESE" ]

    let result1 =
        sortedWaypointNames (addWaypointsToShoppingList shoppingListWithDepartment waypoints1)

    Assert.AreEqual(expected1, result1)

    let expected2 = [ "CHEESE"; "JUICES" ]

    let result2 =
        sortedWaypointNames (addWaypointsToShoppingList shoppingListWithDepartment waypoints2)

    Assert.AreEqual(expected2, result2)

[<Test>]
let ``should return ShoppingList with sorted items`` () =
    let expected: ShoppingList =
        { Id = shoppingList.Id
          Name = shoppingList.Name
          Password = shoppingList.Password
          Items =
              [ { Id = 0
                  Name = "Orange juice"
                  Quantity = 1
                  ItemType = ItemType.ToBuy }
                { Id = 2
                  Name = "Cheddar cheese"
                  Quantity = 1
                  ItemType = ItemType.ToBuy }
                { Id = 1
                  Name = "Carrot"
                  Quantity = 2
                  ItemType = ItemType.ToBuy } ] }

    let result = sortShoppingListItems shoppingList

    Assert.AreEqual(expected, result)
