using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Dtos
{
    public class ShoppingListGetRequest
    {
        [Required] public int Id { get; set; }
        [Required] public string Password { get; set; }

        public ShoppingListGetRequest(int id, string password)
        {
            Id = id;
            Password = password;
        }
    }
}