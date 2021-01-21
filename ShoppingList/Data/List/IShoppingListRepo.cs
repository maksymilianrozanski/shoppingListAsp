using LaYumba.Functional;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using ShoppingData;
using ItemDataActionDto = SharedTypes.Dtos.Protected.ItemDataActionDto;

namespace ShoppingList.Data.List
{
    public interface IShoppingListRepo
    {
        Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList);

        Option<ShoppingListReadDto> GetShoppingListReadDtoById(int id);

        public Option<ShoppingListReadDto> GetShoppingListReadDtoByIdWithSorting(int id);

        Either<ShoppingListErrors.ShoppingListErrors, int> PasswordMatchesShoppingList(int shoppingListId,
            string password);

        Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> AddItemToShoppingListDto(
            Option<ItemDataCreateDto> itemToAdd);

        Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> ModifyShoppingList(
            Option<ItemDataActionDto> itemDataAction);

        bool SaveChanges();
    }
}