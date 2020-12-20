using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Dtos
{
    public class ItemDataCreateDto2
    {
        [Required] public int ShoppingListId { get; set; }
        
        [Required] public string Name { get; set; }

        [Required] public int Quantity { get; set; }

        [Required] public string ItemType { get; set; }
    }
}