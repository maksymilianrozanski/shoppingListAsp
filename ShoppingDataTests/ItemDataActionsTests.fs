module ItemDataActionsTests

open NUnit.Framework
open ShoppingData.ShoppingItemModule

open NUnit.Framework

[<SetUp>]
let Setup () = ()

[<Test>]
let Test1 () = Assert.Pass()

[<Test>]
let ``should change ToBuy type to Assigned`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = ToBuy }

    let expected =
        { someItem with
              ItemType = Assigned("J") }

    let types = [ ToBuy; NotFound ]
    for t in types do
        let item = { someItem with ItemType = t }
        let result = assignItem item "J"
        Assert.AreEqual(expected, result)

[<Test>]
let ``assignItem should not change item with ItemTypes Assigned; Bought; Cancelled`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = NotFound }

    let types = [ Assigned("John"); Bought; Cancelled ]
    for t in types do
        let item = { someItem with ItemType = t }
        let result = assignItem item "J"
        Assert.AreSame(item, result)

[<Test>]
let ``should not change item when notFoundItem called on item without Assigned ItemType`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M") }

    let otherTypes = [ ToBuy; Bought; NotFound; Cancelled ]
    for i in otherTypes do
        let item = { someItem with ItemType = i }
        let result = notFoundItem item "MM"
        Assert.AreSame(item, result)

[<Test>]
let ``should change ItemType from Assigned to NotFound when matched username `` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M") }

    let expected = { someItem with ItemType = NotFound }
    let result = notFoundItem someItem "M"
    Assert.AreEqual(expected, result)

[<Test>]
let ``should not change ItemType from Assigned if username not matched`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M") }

    let result = notFoundItem someItem "MM"
    Assert.AreSame(someItem, result)

[<Test>]
let ``toBought should change ItemType from Assigned to Bought if username matched`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M") }

    let expected = { someItem with ItemType = Bought }
    let result = toBought someItem "M"
    Assert.AreEqual(expected, result)

[<Test>]
let ``toBought should not change ItemType from Assigned when username not matched`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M") }

    let result = toBought someItem "other name"
    Assert.AreSame(someItem, result)

[<Test>]
let ``toBought should not change ItemType from Assigned for ItemTypes other than Assigned`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M") }

    let otherTypes = [ ToBuy; Bought; NotFound; Cancelled ]
    for t in otherTypes do
        let item = { someItem with ItemType = t }
        let result = toBought item "name"
        Assert.AreSame(item, result)

[<Test>]
let ``toCancelled should change ItemType to Cancelled`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M") }

    let types =
        [ ToBuy
          Assigned("M")
          Bought
          NotFound
          Cancelled ]

    let expected = { someItem with ItemType = Cancelled }
    for t in types do
        let item = { someItem with ItemType = t }
        let result = toCancelled item
        Assert.AreEqual(expected, result)
