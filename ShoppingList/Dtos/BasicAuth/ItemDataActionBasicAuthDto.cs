using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Dtos.BasicAuth
{
    public class ItemDataActionBasicAuthDto
    {
        [Required] public int ItemId { get; set; }
        [Required] public int ShoppingListId { get; set; }
        [Required] public int ActionNumber { get; set; }

        public ItemDataActionDto ToItemDataActionDto((string, string) namePassword) =>
            new ItemDataActionDto(namePassword.Item1, ItemId, ShoppingListId, namePassword.Item2, ActionNumber);
    }
}