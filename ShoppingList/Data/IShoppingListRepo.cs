using LaYumba.Functional;
using ShoppingData;
using ShoppingList.Dtos;
using ShoppingList.Dtos.Protected;

namespace ShoppingList.Data
{
    public interface IShoppingListRepo
    {
        Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList);

        Option<ShoppingListReadDto> GetShoppingListReadDtoById(int id);

        public Option<ShoppingListReadDto> GetShoppingListReadDtoByIdWithSorting(int id);

        Either<ShoppingListErrors.ShoppingListErrors, int> PasswordMatchesShoppingList(int shoppingListId,
            string password);

        Either<string, ShoppingListReadDto> AddItemToShoppingListNoPassword(
            Option<ItemDataCreateDtoNoPassword> itemToAdd);

        Either<string, ShoppingListReadDto> ModifyShoppingListItemNoPassword(
            Option<ItemDataActionDtoNoPassword> itemDataAction);

        bool SaveChanges();
    }
}