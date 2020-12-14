using LaYumba.Functional;
using ShoppingList.Dtos;

namespace ShoppingList.Data
{
    public interface IShoppingListRepo
    {
        Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList);

        Option<ShoppingListReadDto> GetShoppingListEntityById(int id);

        Option<ShoppingListReadDto> UpdateShoppingListEntity(Option<ShoppingListUpdateDto> updatedList);

        Option<ShoppingListReadDto> AddItemToShoppingList(Option<ItemDataCreateDto> itemToAdd);

        Option<ShoppingListReadDto> ModifyShoppingListItem(Option<ItemDataActionDto> itemDataAction);

        bool SaveChanges();
    }
}