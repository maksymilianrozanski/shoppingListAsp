using LaYumba.Functional;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using ShoppingData;

namespace ShoppingList.Data.List
{
    public interface IShoppingListRepo
    {
        Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList);

        Option<ShoppingListReadDto> GetShoppingListReadDtoById(int id);

        public Option<ShoppingListReadDto> GetShoppingListReadDtoByIdWithSorting(int id);

        Either<ShoppingListErrors.ShoppingListErrors, int> PasswordMatchesShoppingList(int shoppingListId,
            string password);

        Either<Error, ShoppingListReadDto> AddItemToShoppingListDto(
            Option<ItemDataCreateDto> itemToAdd);

        Either<Error, ShoppingListReadDto> ModifyShoppingList(
            Option<ItemDataActionDtoNoPassword> itemDataAction);

        bool SaveChanges();
    }
}