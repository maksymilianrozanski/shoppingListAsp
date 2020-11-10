module ShoppingDataTests.ShoppingListTests

open NUnit.Framework
open ShoppingData.ShoppingListModule
open ShoppingData.ShoppingItemModule
open ShoppingData

[<SetUp>]
let Setup () = ()

let private shoppingList = emptyShoppingList "MyList" "pass"
let private modName (list: ShoppingList) = { list with Name = "modifiedName" }

[<Test>]
let ``should execute function if password is correct`` () =
    let expected = emptyShoppingList "modifiedName" "pass"

    let result =
        executeIfPassword shoppingList "pass" modName

    match result with
    | Choice1Of2 (r) -> Assert.AreEqual(expected, r)
    | _ -> failwith ("should match to Choice1Of2")


[<Test>]
let ``should return Choice1Of2(IncorrectPassword) error if password is not correct`` () =
    let result =
        executeIfPassword shoppingList "badPass" modName

    match result with
    | Choice2Of2 (f) -> Assert.AreEqual(IncorrectPassword, f)
    | _ -> failwith ("should match to Choice1Of2")

[<Test>]
let ``should return list with item added`` () =
    let milk =
        { Name = "Milk"
          Quantity = 2
          ItemType = ToBuy }

    let coffee =
        { Name = "Coffee"
          Quantity = 4
          ItemType = ToBuy }

    let initial =
        { Name = "myList"
          Password = "pass"
          Items = [ milk ] }


    let expected =
        { initial with
              Items = [ milk; coffee ] }

    let result = addItem initial coffee

    Assert.AreEqual(expected, result)
