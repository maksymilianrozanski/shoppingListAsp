module ShoppingDataTests.ShoppingListTests

open NUnit.Framework
open ShoppingData.ShoppingListModule
open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListErrors

[<SetUp>]
let Setup () = ()

let private shoppingList = emptyShoppingList 11 "pass"

let private modifyId (list: ShoppingList) =
    Choice1Of2({ list with Id = 120 })

let milk =
    { Name = "Milk"
      Quantity = 2
      ItemType = ToBuy
      Id = 0 }

let coffee =
    { Name = "Coffee"
      Quantity = 4
      ItemType = ToBuy
      Id = 1 }

let chocolate =
    { Name = "Chocolate"
      Quantity = 1
      ItemType = ToBuy
      Id = 2 }


[<Test>]
let ``should return list with item added`` () =
    let initial =
        { 
          Password = "pass"
          Items = [ milk ]
          Id = 11
          ShopName = "shopName" }

    let expected =
        { initial with
              Items = [ milk; coffee ] }

    let result = addItem initial coffee

    Assert.AreEqual(expected, result)

let threeItemList =
    { 
      Password = "pass"
      Items = [ milk; coffee; chocolate ]
      Id = 11
      ShopName = "shopName" }

[<Test>]
let ``should update coffee item`` () =
    let coffeeBought = { coffee with ItemType = Bought }
    let f _ = Choice1Of2(coffeeBought)

    let expected: Choice<ShoppingList, ShoppingListErrors> =
        Choice1Of2
            ({ threeItemList with
                   Items = [ milk; coffeeBought; chocolate ] })

    let result = modifyItem f 1 threeItemList
    Assert.AreEqual(expected, result)

[<Test>]
let ``should return Choice2Of2(ListItemNotFound)`` () =
    let f _ = failwith ("should not be called")

    let expected: Choice<ShoppingList, ShoppingListErrors> = Choice2Of2(ListItemNotFound)

    let result = modifyItem f 3 threeItemList
    Assert.AreEqual(expected, result)

[<Test>]
let ``should return Choice2Of2(ForbiddenOperation)`` () =
    let f _ = Choice2Of2(ForbiddenOperation)

    let expected: Choice<ShoppingList, ShoppingListErrors> = Choice2Of2(ForbiddenOperation)

    let result = modifyItem f 1 threeItemList
    Assert.AreEqual(expected, result)

let bread id =
    { Name = "Bread"
      Quantity = 1
      ItemType = ToBuy
      Id = id }

[<Test>]
let ``should not add item to the list when Items contains item with Id`` () =
    let expected: Choice<ShoppingList, ShoppingListErrors> = Choice2Of2(ItemWithIdAlreadyExists)

    let result =
        addItemIfNotExist (bread 1) threeItemList

    Assert.AreEqual(expected, result)

[<Test>]
let ``should add item to the list when Items does not contain item with Id`` () =
    let itemToAdd = (bread 3)

    let expected: Choice<ShoppingList, ShoppingListErrors> =
        Choice1Of2(addItem threeItemList itemToAdd)

    let result =
        addItemIfNotExist itemToAdd threeItemList

    Assert.AreEqual(expected, result)