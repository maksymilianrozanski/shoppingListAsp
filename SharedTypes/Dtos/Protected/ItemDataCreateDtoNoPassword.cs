using System.ComponentModel.DataAnnotations;

namespace SharedTypes.Dtos.Protected
{
    public class ItemDataCreateDtoNoPassword
    {
        public ItemDataCreateDtoNoPassword(int shoppingListId, string name, int quantity)
        {
            ShoppingListId = shoppingListId;
            Name = name;
            Quantity = quantity;
        }

        [Required] public int ShoppingListId { get; set; }

        [Required] public string Name { get; set; }

        [Required] public int Quantity { get; set; }
    }
}