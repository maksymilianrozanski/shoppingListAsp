module ShoppingDataTests.ItemDataActionsTests

open NUnit.Framework
open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListErrors

[<SetUp>]
let Setup () = ()

[<Test>]
let Test1 () = Assert.Pass()

[<Test>]
let ``should change ToBuy type to Assigned`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = ToBuy
          Id = 0 }

    let expected =
        { someItem with
              ItemType = Assigned("J") }

    let types = [ ToBuy; NotFound ]
    for t in types do
        let item = { someItem with ItemType = t }
        let result = assignItem "J" item
        match result with
        | Choice1Of2 (x) -> Assert.AreEqual(expected, x)
        | _ -> failwith ("should match to Choice1Of2")

[<Test>]
let ``assignItem should not change item with ItemTypes Assigned; Bought; Cancelled`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = NotFound
          Id = 0 }

    let types = [ Assigned("John"); Bought; Cancelled ]
    for t in types do
        let item = { someItem with ItemType = t }
        let result = assignItem "J" item
        match result with
        | Choice2Of2 (x) -> Assert.AreEqual(ForbiddenOperation, x)
        | _ -> failwith ("should match to Choice2Of2")

[<Test>]
let ``should return ForbiddenOperation when notFoundItem called on item without Assigned ItemType`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M")
          Id = 0 }

    let otherTypes = [ ToBuy; Bought; NotFound; Cancelled ]
    for i in otherTypes do
        let item = { someItem with ItemType = i }
        let result = notFoundItem "MM" item
        match result with
        | Choice2Of2 (x) -> Assert.AreSame(ForbiddenOperation, x)
        | _ -> failwith ("should match to Choice2Of2")

[<Test>]
let ``should return item with changed ItemType from Assigned to NotFound when matched username `` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M")
          Id = 0 }

    let expected = { someItem with ItemType = NotFound }
    let result = notFoundItem "M" someItem
    match result with
    | Choice1Of2 (x) -> Assert.AreEqual(expected, x)
    | _ -> failwith ("should match to Choice1Of2")

[<Test>]
let ``should return Choice2Of2(IncorrectUser) if username not matched`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M")
          Id = 0 }

    let result = notFoundItem "MM" someItem
    match result with
    | Choice2Of2 (x) -> Assert.AreEqual(IncorrectUser, x)
    | _ -> failwith ("should match to Choice2Of2")

[<Test>]
let ``toBought should return item with changed ItemType from Assigned to Bought if username matched`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M")
          Id = 0 }

    let expected = { someItem with ItemType = Bought }
    let result = toBought "M" someItem
    match result with
    | Choice1Of2 (x) -> Assert.AreEqual(expected, x)
    | _ -> failwith ("should match to Choice1Of2")

[<Test>]
let ``toBought should return Choice2Of2(IncorrectUser) when username not matched`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M")
          Id = 0 }

    let result = toBought "other name" someItem
    match result with
    | Choice2Of2 (x) -> Assert.AreEqual(IncorrectUser, x)
    | _ -> failwith ("should match to Choice2Of2")

[<Test>]
let ``toBought should return Choice2Of2(ForbiddenOperation) for items with ItemType other than Assigned`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M")
          Id = 0 }

    let otherTypes = [ ToBuy; Bought; NotFound; Cancelled ]
    for t in otherTypes do
        let item = { someItem with ItemType = t }
        let result = toBought "name" item
        match result with
        | Choice2Of2 (x) -> Assert.AreEqual(ForbiddenOperation, x)
        | _ -> failwith ("should match to Choice2Of2")

[<Test>]
let ``toCancelled should return item with changed ItemType to Cancelled`` () =
    let someItem =
        { Name = "Sugar"
          Quantity = 10
          ItemType = Assigned("M")
          Id = 0 }

    let types =
        [ ToBuy
          Assigned("M")
          Bought
          NotFound
          Cancelled ]

    let expected =
        Choice1Of2({ someItem with ItemType = Cancelled })

    for t in types do
        let item = { someItem with ItemType = t }
        let result = toCancelled "not-used" item
        Assert.AreEqual(expected, result)
