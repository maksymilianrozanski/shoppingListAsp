module ShoppingDataTests.ShoppingListTests

open NUnit.Framework
open ShoppingData.ShoppingListModule
open ShoppingData.Utils

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
    | Success (r) -> Assert.AreEqual(expected, r)
    | _ -> failwith ("should have matched to success")


[<Test>]
let ``should return error if password is not correct`` () =
    let expected = IncorrectPassword

    let result =
        executeIfPassword shoppingList "badPass" modName

    match result with
    | Failure (f) -> Assert.AreEqual(expected, f)
    | _ -> failwith ("should have matched to failure")
