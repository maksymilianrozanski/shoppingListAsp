module ShoppingDataTests.ShoppingListTests

open NUnit.Framework
open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListModule

[<SetUp>]
let Setup () = ()

let shoppingList = emptyShoppingList "MyList" "pass"

[<Test>]
let ``should execute function if password is correct`` () =
    let f (list: ShoppingList) = { list with Name = "modifiedName" }
    let expected = emptyShoppingList "modifiedName" "pass"
    let result = executeIfPassword shoppingList "pass" f
    Assert.AreEqual(expected, result)
