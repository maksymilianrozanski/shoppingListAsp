module ShoppingListSortingTests.WaypointsOrderTests

open NUnit.Framework
open SharedTypes.Dtos
open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListModule
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
        sortedWaypointNames (addWaypointsToShoppingList waypoints1 shoppingListWithDepartment)

    Assert.AreEqual(expected1, result1)

    let expected2 = [ "CHEESE"; "JUICES" ]

    let result2 =
        sortedWaypointNames (addWaypointsToShoppingList waypoints2 shoppingListWithDepartment)

    Assert.AreEqual(expected2, result2)

[<Test>]
let ``should return ShoppingList with sorted items`` () =
    let expected: ShoppingList =
        { Id = shoppingList.Id
          Name = shoppingList.Name
          Password = shoppingList.Password
          ShopName = shoppingList.ShopName
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

[<Test>]
let ``should return ShoppingList with sorted items, when contains multiple items at one waypoint`` () =
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

                ({ Id = 4
                   Name = "Pineapple"
                   Quantity = 3
                   ItemType = ItemType.ToBuy },
                 PredictedShopsDepartment("FRUITS"),
                 None)

                ({ Id = 3
                   Name = "Apple juice"
                   Quantity = 1
                   ItemType = ItemType.ToBuy },
                 PredictedShopsDepartment("JUICES"),
                 Some(waypoints1.Waypoints.Item(0)))

                ({ Id = 5
                   Name = "Goat cheese"
                   Quantity = 1
                   ItemType = ItemType.ToBuy },
                 PredictedShopsDepartment("CHEESE"),
                 Some(waypoints1.Waypoints.Item(2)))

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

    let result = sortShoppingListItems shoppingList

    let resultItems = result.Items

    let expected0 =
        { Id = 0
          Name = "Orange juice"
          Quantity = 1
          ItemType = ItemType.ToBuy }

    let expected1 =
        { Id = 3
          Name = "Apple juice"
          Quantity = 1
          ItemType = ItemType.ToBuy }

    let expected2 =
        { Id = 5
          Name = "Goat cheese"
          Quantity = 1
          ItemType = ItemType.ToBuy }

    let expected3 =
        { Id = 2
          Name = "Cheddar cheese"
          Quantity = 1
          ItemType = ItemType.ToBuy }

    let expected4 =
        { Id = 4
          Name = "Pineapple"
          Quantity = 3
          ItemType = ItemType.ToBuy }

    let expected5 =
        { Id = 1
          Name = "Carrot"
          Quantity = 2
          ItemType = ItemType.ToBuy }

    Assert.AreEqual(shoppingList.Items.Length, resultItems.Length)

    Assert.True
        (expected0 = resultItems.[0]
         || expected1 = resultItems.[0],
         "index-0 and index-1 items are expected to be matched to 'JUICES' waypoint")

    Assert.True
        (expected0 = resultItems.[1]
         || expected1 = resultItems.[1],
         "index-0 and index-1 items are expected to be matched to 'JUICES' waypoint")

    Assert.True
        (expected2 = resultItems.[2]
         || expected3 = resultItems.[2],
         "index-2 and index-3 items are expected to be matched to 'CHEESE' waypoint")

    Assert.True
        (expected2 = resultItems.[3]
         || expected3 = resultItems.[3],
         "index-2 and index-3 items are expected to be matched to 'CHEESE' waypoint")

    Assert.True
        (expected4 = resultItems.[4]
         || expected5 = resultItems.[4],
         "index-4 and index-5 items not matched to any waypoint are expected to be placed at the end of the list")

    Assert.True
        (expected4 = resultItems.[5]
         || expected5 = resultItems.[5],
         "index-4 and index-5 items not matched to any waypoint are expected to be placed at the end of the list")

    Assert.AreEqual(resultItems.Length, (List.distinct resultItems).Length, "all items in the list should be unique")
