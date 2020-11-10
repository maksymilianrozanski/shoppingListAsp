namespace ShoppingData

module ShoppingItemModule =
    let hello name = printfn "Hello %s" name

    type ItemType =
        | ToBuy
        | Assigned of string
        | Bought
        | NotFound
        | Cancelled

    type ItemData =
        { Name: string
          Quantity: int
          ItemType: ItemType }

    let ifThisUser (item: ItemData) user f =
        match item.ItemType with
        | Assigned(u) -> if (u = user) then Choice1Of2((f item)) else Choice2Of2(IncorrectUser)
        | _ -> Choice2Of2(ForbiddenOperation)

    let assignItem item user =
        match item.ItemType with
        | ToBuy
        | NotFound -> Choice1Of2({ item with ItemType = Assigned(user) })
        | _ -> Choice2Of2(ForbiddenOperation)

    let notFoundItem item user =
        match item.ItemType with
        | Assigned (_) -> ifThisUser item user (fun i -> { i with ItemType = NotFound })
        | _ -> Choice2Of2(ForbiddenOperation)

    let toBought item user =
        match item.ItemType with
        | Assigned (_) -> ifThisUser item user (fun i -> { i with ItemType = Bought })
        | _ -> Choice2Of2(ForbiddenOperation)

    let toCancelled item = { item with ItemType = Cancelled }
