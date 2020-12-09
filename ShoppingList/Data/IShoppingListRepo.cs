using ShoppingList.Dtos;
using ShoppingList.Entities;

namespace ShoppingList.Data
{
    public interface IShoppingListRepo
    {
        ShoppingListEntity CreateShoppingList(ShoppingListCreateDto shoppingList);

        ShoppingListEntity GetShoppingListEntityById(int id);
        
        bool SaveChanges();
    }
}