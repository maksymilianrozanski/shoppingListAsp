namespace ShoppingData

open LaYumba.Functional

module ShoppingListErrors =
    type ShoppingListErrors =
        | IncorrectPassword
        | IncorrectUser
        | ForbiddenOperation
        | ListItemNotFound
        | ItemWithIdAlreadyExists
        | OtherError of Error
