module ShoppingDataTests.IfThisUserTests

open ShoppingData.ShoppingItemModule

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
          ItemType = Assigned("J") }

    let f item = { item with Name = "Brown sugar" }

    let expected =
        { Name = "Brown sugar"
          Quantity = 10
          ItemType = Assigned("J") }

    let result = ifThisUser (data) "J" f
    Assert.AreEqual(expected, result)

[<Test>]
let ``should not apply function if ItemType different than Assigned`` () =
    let data =
        { Name = "Sugar"
          Quantity = 10
          ItemType = ToBuy }

    let f item = { item with Name = "Brown sugar" }
    let expected = data
    let result = ifThisUser data "J" f
    Assert.AreSame(expected, result)

[<Test>]
let ``should not apply function if provided username if different than item's`` () =
    let data =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("J") }

    let f item = { item with Name = "Brown sugar" }
    let expected = data
    let result = ifThisUser data "K" f
    Assert.AreSame(expected, result)
