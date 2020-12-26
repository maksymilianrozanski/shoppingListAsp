using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Dtos.Protected
{
    public class ItemDataCreateDtoNoPassword
    {
        [Required] public int ShoppingListId { get; set; }

        [Required] public string Name { get; set; }

        [Required] public int Quantity { get; set; }

        public ItemDataCreateDtoNoPassword(int shoppingListId, string name, int quantity)
        {
            ShoppingListId = shoppingListId;
            Name = name;
            Quantity = quantity;
        }
    }
}