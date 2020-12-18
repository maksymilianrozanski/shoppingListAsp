using LaYumba.Functional;
using ShoppingList.Dtos;

namespace ShoppingList.Data
{
    public interface IShoppingListRepo
    {
        Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList);

        Option<ShoppingListReadDto> GetShoppingListEntityById(int id);

        Either<RepoRequestError, ShoppingListReadDto> GetShoppingListEntityByIdIfPassword(Option<ShoppingListGetRequest> request);

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