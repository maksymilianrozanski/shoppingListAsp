namespace ShoppingData

open LaYumba.Functional

module ShoppingListErrors =
    type ShoppingListErrors =
        | LoginAlreadyExists
        | CreatingUserFailed
        | IncorrectPassword
        | IncorrectUser
        | ForbiddenOperation
        | NotFound
        | ItemWithIdAlreadyExists
        | ShopNotFound
        | UnknownDbError
        | OtherError of Error
