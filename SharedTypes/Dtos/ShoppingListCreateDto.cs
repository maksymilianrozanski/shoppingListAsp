using System.ComponentModel.DataAnnotations;

namespace SharedTypes.Dtos
{
    public class ShoppingListCreateDto
    {
        public ShoppingListCreateDto(string shoppingListName, string password, string shopName)
        {
            Name = shoppingListName;
            Password = password;
            ShopName = shopName;
        }

        [Required] [MaxLength(100)] public string Name { get; set; }
        [Required] [MaxLength(20)] public string Password { get; set; }
        [MaxLength(100)] public string ShopName { get; set; }
    }
}