namespace ShoppingData

module ShoppingListErrors =
    type ShoppingListErrors =
        | IncorrectPassword
        | IncorrectUser
        | ForbiddenOperation
        | ListItemNotFound
        | ItemWithIdAlreadyExists
