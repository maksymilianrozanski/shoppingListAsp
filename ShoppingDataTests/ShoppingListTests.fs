module ShoppingDataTests.ShoppingListTests

open NUnit.Framework
open ShoppingData.ShoppingListModule
open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListErrors

[<SetUp>]
let Setup () = ()

let private shoppingList = emptyShoppingList "MyList" 11 "pass"

let private modName (list: ShoppingList) =
    Choice1Of2({ list with Name = "modifiedName" })

[<Test>]
let ``should execute function if password is correct`` () =
    let expected =
        emptyShoppingList "modifiedName" 11 "pass"

    let result =
        executeIfPassword modName shoppingList "pass"

    match result with
    | Choice1Of2 (r) -> Assert.AreEqual(expected, r)
    | _ -> failwith ("should match to Choice1Of2")


[<Test>]
let ``should return Choice1Of2(IncorrectPassword) error if password is not correct`` () =
    let result =
        executeIfPassword modName shoppingList "badPass"

    match result with
    | Choice2Of2 (f) -> Assert.AreEqual(IncorrectPassword, f)
    | _ -> failwith ("should match to Choice1Of2")

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
        { Name = "myList"
          Password = "pass"
          Items = [ milk ]
          Id = 11 }

    let expected =
        { initial with
              Items = [ milk; coffee ] }

    let result = addItem initial coffee

    Assert.AreEqual(expected, result)

let threeItemList =
    { Name = "myList"
      Password = "pass"
      Items = [ milk; coffee; chocolate ]
      Id = 11 }

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

[<Test>]
let ``modifyItemIfPassword should return modified list when password is correct`` () =
    let coffeeBought = { coffee with ItemType = Bought }
    let f (_: ItemData) = Choice1Of2(coffeeBought)

    let expected: Choice<ShoppingList, ShoppingListErrors> =
        Choice1Of2
            ({ threeItemList with
                   Items = [ milk; coffeeBought; chocolate ] })

    let result =
        modifyItemIfPassword f 1 threeItemList "pass"

    Assert.AreEqual(expected, result)

[<Test>]
let ``modifyItemIfPassword should return Choice2Of2(IncorrectPassword) when password is not correct`` () =
    let f (_: ItemData) = failwith ("should not be called")

    let expected: Choice<ShoppingList, ShoppingListErrors> = Choice2Of2(IncorrectPassword)

    let result =
        modifyItemIfPassword f 1 threeItemList "incorrect password"

    Assert.AreEqual(expected, result)

[<Test>]
let ``modifyItemIfPassword should return Choice2Of2 of f function`` () =
    let f _ = Choice2Of2(ForbiddenOperation)
    let expected: Choice<ShoppingList, ShoppingListErrors> = Choice2Of2(ForbiddenOperation)

    let result =
        modifyItemIfPassword f 1 threeItemList "pass"

    Assert.AreEqual(expected, result)

[<Test>]
let ``should return list with item's ItemType assigned to user`` () =
    let expected: Choice<ShoppingList, ShoppingListErrors> =
        Choice1Of2
            ({ threeItemList with
                   Items =
                       [ milk
                         { coffee with
                               ItemType = Assigned("John") }
                         chocolate ] })

    let result =
        listItemToAssigned "John" 1 threeItemList "pass"

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

[<Test>]
let ``should add item to the list when password is correct and Items does not contain item with Id`` () =
    let itemToAdd = (bread 3)

    let expected: Choice<ShoppingList, ShoppingListErrors> =
        Choice1Of2(addItem threeItemList itemToAdd)

    let result =
        addItemIfPassword itemToAdd threeItemList "pass"

    Assert.AreEqual(expected, result)

[<Test>]
let ``should return Choice2Of2(ItemWithIdAlreadyExists) when password is correct and Items contains item with Id`` () =
    let itemToAdd = (bread 1)
    let expected: Choice<ShoppingList, ShoppingListErrors> = Choice2Of2(ItemWithIdAlreadyExists)

    let result =
        addItemIfPassword itemToAdd threeItemList "pass"

    Assert.AreEqual(expected, result)

[<Test>]
let ``should return Choice2Of2(IncorrectPassword) when password is incorrect and Items contains item with Id`` () =
    let itemToAdd = (bread 1)
    let expected: Choice<ShoppingList, ShoppingListErrors> = Choice2Of2(IncorrectPassword)

    let result =
        addItemIfPassword itemToAdd threeItemList "incorrect"

    Assert.AreEqual(expected, result)

[<Test>]
let ``should return Choice2Of2(IncorrectPassword) when password is incorrect and Items does not contain item with Id`` () =
    let itemToAdd = (bread 1)
    let expected: Choice<ShoppingList, ShoppingListErrors> = Choice2Of2(IncorrectPassword)

    let result =
        addItemIfPassword itemToAdd threeItemList "incorrect"

    Assert.AreEqual(expected, result)
