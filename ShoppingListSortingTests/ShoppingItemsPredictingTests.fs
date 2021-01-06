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

    let shoppingList =
        { Id = 0
          Name = "My-shopping-list"
          Password = "pass"
          Items =
              [ { Id = 0
                  Name = "Orange juice"
                  Quantity = 1
                  ItemType = ToBuy }

                { Id = 1
                  Name = "Carrot"
                  Quantity = 2
                  ItemType = ToBuy } ] }

    let zipped1 =
        ({ Id = 0
           Name = "Orange juice"
           Quantity = 1
           ItemType = ItemType.ToBuy },
         PredictedShopsDepartment("JUICES"))

    let zipped2: ItemDataWithPredictedType =
        ({ Id = 1
           Name = "Carrot"
           Quantity = 2
           ItemType = ItemType.ToBuy },
         PredictedShopsDepartment("VEGETABLES"))

    let itemsTransformed: List<ItemDataWithPredictedType> =
        ResizeArray<ItemDataWithPredictedType> [ zipped1; zipped2 ]

    let expected: ShoppingListWithDepartment =
        { Id = 0
          Name = "My-shopping-list"
          Password = "pass"
          ShopName = "todo"
          Items = itemsTransformed }

    let result =
        predictAllItems predictSingleItemMock shoppingList

    //todo: add assertion
    Assert.AreEqual(2, 2)
