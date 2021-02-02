module ShoppingListSortingTests.ShoppingItemsPredictingTests

open NUnit.Framework
open NUnit.Framework.Internal
open ShoppingData
open ShoppingListSorting
open ShoppingItemModule
open ShoppingListModule
open ShoppingItemsPredicting
open System.Collections.Generic

[<SetUp>]
let Setup () = ()

[<Test>]
let Test1 () = Assert.Pass()

[<Test>]
let ``should add predicted values to shopping list`` () =

    let predictSingleItemMock (item: ItemData) =
        if (item.Id = 0) then "JUICES" else "VEGETABLES"

    let shoppingListInput: ShoppingList =
        { Id = 0
          Password = "pass"
          ShopName = "shopName"
          Items =
              [ { Id = 0
                  Name = "Orange juice"
                  Quantity = 1
                  ItemType = ToBuy }
                { Id = 1
                  Name = "Carrot"
                  Quantity = 2
                  ItemType = ToBuy } ] }

    let expected =
        { Id = 0
          Password = "pass"
          ShopName = "shopName"
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
                     PredictedShopsDepartment("VEGETABLES")) ] }

    let result =
        predictAllItems predictSingleItemMock shoppingListInput

    Assert.AreEqual(expected.Id, result.Id)
    Assert.AreEqual(expected.ShopName, result.ShopName)
    Assert.AreEqual(expected.Items, result.Items)
