using ShoppingList.Dtos;

namespace ShoppingList.Data
{
    public interface IShoppingListRepo
    {
        void CreateShoppingList(ShoppingListCreateDto shoppingList);

        bool SaveChanges();
    }
}