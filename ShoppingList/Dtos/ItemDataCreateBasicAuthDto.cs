using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Dtos
{
    public class ItemDataCreateBasicAuthDto
    {
        [Required] public int ShoppingListId { get; set; }

        [Required] public string Name { get; set; }

        [Required] public int Quantity { get; set; }

        [Required] public string ItemType { get; set; }

        public ItemDataCreateDto ToItemDataCreateDto(string password) =>
            new ItemDataCreateDto(this.ShoppingListId, password, this.Name,
                this.Quantity, this.ItemType);
    }
}