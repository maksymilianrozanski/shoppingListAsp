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

    let ifThisUser (item: ItemData) user f: ItemData =
        match item.ItemType with
        | Assigned (u) -> if (u = user) then (f item) else item
        | _ -> item

    let assignItem item user =
        match item.ItemType with
        | ToBuy
        | NotFound -> { item with ItemType = Assigned(user) }
        | _ -> item

    let notFoundItem item user =
        match item.ItemType with
        | Assigned (_) -> ifThisUser item user (fun i -> { i with ItemType = NotFound })
        | _ -> item

    let toBought item user =
        match item.ItemType with
        | Assigned (_) -> ifThisUser item user (fun i -> { i with ItemType = Bought })
        | _ -> item

    let toCancelled item = { item with ItemType = Cancelled }
