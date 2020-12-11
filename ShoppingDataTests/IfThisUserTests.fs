module ShoppingDataTests.IfThisUserTests

open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListErrors

open NUnit.Framework

[<SetUp>]
let Setup () = ()

[<Test>]
let Test1 () = Assert.Pass()

[<Test>]
let ``should apply function only if ItemType is Assigned and match username`` () =
    let data =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("J")
          Id = 0 }

    let f item = { item with Name = "Brown sugar" }

    let expected =
        { Name = "Brown sugar"
          Quantity = 10
          ItemType = Assigned("J")
          Id = 0 }

    let result = ifThisUser (data) "J" f
    match result with
    | Choice1Of2 (x) -> Assert.AreEqual(expected, x)
    | _ -> failwith ("should match to Choice1Of2")

[<Test>]
let ``should return Choice2Of2(ForbiddenOperation) if ItemType other than Assigned`` () =
    let data =
        { Name = "Sugar"
          Quantity = 10
          ItemType = ToBuy
          Id = 0 }

    let f item = { item with Name = "Brown sugar" }
    let result = ifThisUser data "J" f
    match result with
    | Choice2Of2(x) -> Assert.AreEqual(ForbiddenOperation, x)
    | _ -> failwith("should match to Choice2Of2")

[<Test>]
let ``should return Choice2Of2(IncorrectUser) if provided username is different than item's`` () =
    let data =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("J")
          Id = 0 }

    let f item = { item with Name = "Brown sugar" }
    let result = ifThisUser data "K" f
    match result with
    | Choice2Of2(x) -> Assert.AreEqual(IncorrectUser, x)
    | _ -> failwith("should match to Choice2Of2")