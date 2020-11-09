namespace ShoppingData

open ShoppingData.ShoppingItemModule
open Utils

module ShoppingListModule =

    type ShoppingListErrors = | IncorrectPassword

    type ShoppingList =
        { Name: string
          Password: string
          Items: List<ItemData> }

    let emptyShoppingList name password =
        { Name = name
          Password = password
          Items = List.empty }

    let executeIfPassword (list: ShoppingList) password f =
        if (list.Password = password) then Success(f (list)) else Failure(IncorrectPassword)

    let addItem (list: ShoppingList) item =
        { list with
              Items = list.Items @ [ item ] }
