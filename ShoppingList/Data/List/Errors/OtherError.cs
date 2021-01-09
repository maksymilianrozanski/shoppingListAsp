using LaYumba.Functional;
using Microsoft.FSharp.Core;
using ShoppingData;

namespace ShoppingList.Data.List.Errors
{
    public sealed class OtherError : Error
    {
        public OtherError(ShoppingListErrors.ShoppingListErrors error)
        {
            this.Message = ErrorTextValue(error);
            this.ErrorObject = error;
        }

        public ShoppingListErrors.ShoppingListErrors ErrorObject { get; set; }

        public override string Message { get; }

        private static string ErrorTextValue(ShoppingListErrors.ShoppingListErrors error) =>
            error switch
            {
                var x when x.IsForbiddenOperation => nameof(ShoppingListErrors.ShoppingListErrors.ForbiddenOperation),
                var x when x.IsIncorrectPassword => nameof(ShoppingListErrors.ShoppingListErrors.IncorrectPassword),
                var x when x.IsIncorrectUser => nameof(ShoppingListErrors.ShoppingListErrors.IncorrectUser),
                var x when x.IsListItemNotFound => nameof(ShoppingListErrors.ShoppingListErrors.ListItemNotFound),
                var x when x.IsItemWithIdAlreadyExists => nameof(ShoppingListErrors.ShoppingListErrors
                    .ItemWithIdAlreadyExists),
                _ => throw new MatchFailureException()
            };
    }
}