using LaYumba.Functional;
using ShoppingData;
using ShoppingList.Dtos;
using ShoppingList.Dtos.Protected;

namespace ShoppingList.Data
{
    public interface IShoppingListRepo
    {
        Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList);

        Option<ShoppingListReadDto> GetShoppingListEntityById(int id);

        Either<RepoRequestError, ShoppingListReadDto> GetShoppingListEntityByIdIfPassword(
            Option<ShoppingListGetRequest> request);

        Either<ShoppingListErrors.ShoppingListErrors, int> PasswordMatchesShoppingList(int shoppingListId,
            string password);

        Either<string, ShoppingListReadDto> AddItemToShoppingList(Option<ItemDataCreateDto> itemToAdd);

        Either<string, ShoppingListReadDto> AddItemToShoppingListNoPassword(
            Option<ItemDataCreateDtoNoPassword> itemToAdd);

        Either<string, ShoppingListReadDto> ModifyShoppingListItem(Option<ItemDataActionDto> itemDataAction);

        Either<string, ShoppingListReadDto> ModifyShoppingListItemNoPassword(
            Option<ItemDataActionDtoNoPassword> itemDataAction);

        bool SaveChanges();

        public enum RepoRequestError
        {
            IncorrectPassword,
            NotFound
        }
    }
}