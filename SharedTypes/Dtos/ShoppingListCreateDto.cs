using System.ComponentModel.DataAnnotations;

namespace SharedTypes.Dtos
{
    public class ShoppingListCreateDto
    {
        public ShoppingListCreateDto(string password, string shopName)
        {
            Password = password;
            ShopName = shopName;
        }

        [Required] [MaxLength(20)] public string Password { get; set; }
        [MaxLength(100)] public string ShopName { get; set; }
    }
}