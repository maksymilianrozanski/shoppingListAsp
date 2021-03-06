﻿namespace ShoppingData

module ShoppingItemModule =
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

    let modifyStatus itemType (_: string) item =
        Choice1Of2 { item with ItemType = itemType }
