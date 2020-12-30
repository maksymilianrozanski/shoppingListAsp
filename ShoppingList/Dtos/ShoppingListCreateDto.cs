using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Dtos
{
    public class ShoppingListCreateDto
    {
        public ShoppingListCreateDto(string shoppingListName, string password)
        {
            Name = shoppingListName;
            Password = password;
        }

        [Required] [MaxLength(100)] public string Name { get; set; }
        [Required] [MaxLength(20)] public string Password { get; set; }
    }
}