namespace ShoppingData

open LaYumba.Functional

module ShoppingListErrors =
    type ShoppingListErrors =
        | IncorrectPassword
        | IncorrectUser
        | ForbiddenOperation
        | NotFound
        | ItemWithIdAlreadyExists
        | OtherError of Error
