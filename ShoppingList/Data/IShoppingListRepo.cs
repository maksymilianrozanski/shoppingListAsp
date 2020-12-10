using LaYumba.Functional;
using ShoppingList.Dtos;
using ShoppingList.Entities;

namespace ShoppingList.Data
{
    public interface IShoppingListRepo
    {
        Option<ShoppingListReadDto> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList);

        Option<ShoppingListReadDto> GetShoppingListEntityById(int id);

        Option<ShoppingListReadDto> UpdateShoppingListEntity(Option<ShoppingListUpdateDto> updated);

        bool SaveChanges();
    }
}