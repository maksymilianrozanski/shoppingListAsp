using System.ComponentModel.DataAnnotations;
using ShoppingData;

namespace ShoppingList.Entities
{
    public class ItemDataEntity
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public int Quantity { get; set; }

        [Required] public string ItemType { get; set; }
    }
}