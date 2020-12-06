using ShoppingData;

namespace ShoppingList.Data
{
    public interface IShoppingListRepo
    {
        void CreateShoppingList(ShoppingListModule.ShoppingList shoppingList);
    }
}