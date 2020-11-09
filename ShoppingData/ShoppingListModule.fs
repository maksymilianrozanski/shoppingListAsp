namespace ShoppingData

open ShoppingData.ShoppingItemModule
open Utils

module ShoppingListModule =

    type ShoppingListErrors = | IncorrectPassword

    type ShoppingList =
        { Name: string
          Password: string
          Items: ItemData [] }

    let emptyShoppingList name password =
        { Name = name
          Password = password
          Items = [||] }

    let executeIfPassword (list: ShoppingList) password f =
        if (list.Password = password) then Success(f (list)) else  Failure(IncorrectPassword)
