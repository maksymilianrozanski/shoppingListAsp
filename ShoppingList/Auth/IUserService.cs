using LaYumba.Functional;
using ShoppingData;
using ShoppingList.Data;

namespace ShoppingList.Auth
{
    public interface IUserService<T, TR>
    {
        Option<Either<ShoppingListErrors.ShoppingListErrors, TR>>
            Authenticate(Option<T> user);
    }
}