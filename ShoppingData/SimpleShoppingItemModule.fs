namespace ShoppingData

module SimpleShoppingItemModule =
    type ItemType =
        | ToBuy
        | LookingFor
        | Bought
        | NotFound
        | Cancelled

    type ItemData =
        { Id: int
          Name: string
          Quantity: int
          ItemType: ItemType }

    let modifyStatus item itemType = { item with ItemType = itemType }
