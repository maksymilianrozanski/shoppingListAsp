namespace ShoppingData

open ShoppingData.ShoppingItemModule
open ShoppingData

module ShoppingListModule =

    type ShoppingList =
        { Name: string
          Password: string
          Items: List<ItemData> }

    let emptyShoppingList name password =
        { Name = name
          Password = password
          Items = List.empty }

    let executeIfPassword (list: ShoppingList) password f =
        if (list.Password = password) then Choice1Of2(f (list)) else Choice2Of2(IncorrectPassword)

    let addItem (list: ShoppingList) item =
        { list with
              Items = list.Items @ [ item ] }
