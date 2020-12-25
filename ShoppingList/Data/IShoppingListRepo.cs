using LaYumba.Functional;
using ShoppingData;
using ShoppingList.Dtos;

namespace ShoppingList.Data
{
    public interface IShoppingListRepo
    {
        Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList);

        Either<RepoRequestError, ShoppingListReadDto> GetShoppingListEntityByIdIfPassword(
            Option<ShoppingListGetRequest> request);

        Either<ShoppingListErrors.ShoppingListErrors, int> PasswordMatchesShoppingList(int shoppingListId,
            string password);

        Either<string, ShoppingListReadDto> AddItemToShoppingList(Option<ItemDataCreateDto> itemToAdd);

        Either<string, ShoppingListReadDto> ModifyShoppingListItem(Option<ItemDataActionDto> itemDataAction);

        bool SaveChanges();

        public enum RepoRequestError
        {
            IncorrectPassword,
            NotFound
        }
    }
}