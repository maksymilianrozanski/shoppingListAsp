namespace ShoppingData

type ShoppingListErrors =
        | IncorrectPassword
        | IncorrectUser
        | ForbiddenOperation
        | ListItemNotFound
        | ItemWithIdAlreadyExists