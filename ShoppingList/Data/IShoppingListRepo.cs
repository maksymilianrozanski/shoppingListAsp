using LaYumba.Functional;
using ShoppingList.Dtos;
using ShoppingList.Entities;

namespace ShoppingList.Data
{
    public interface IShoppingListRepo
    {
        Option<ShoppingListEntity> CreateShoppingList(Option<ShoppingListCreateDto> shoppingList);

        Option<ShoppingListEntity> GetShoppingListEntityById(int id);

        bool SaveChanges();
    }
}