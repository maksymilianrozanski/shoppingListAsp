using System.ComponentModel.DataAnnotations;
using Microsoft.FSharp.Core;
using ShoppingData;

namespace SharedTypes.Dtos
{
    public class ItemDataActionDto
    {
        public enum ItemDataActions
        {
            AssignItem = 0,
            ItemToNotFound = 1,
            ItemToBought = 2,
            ItemToCancelled = 3
        }

        public ItemDataActionDto(string user, int itemId, int shoppingListId, string password, int actionNumber)
        {
            User = user;
            ItemId = itemId;
            ShoppingListId = shoppingListId;
            Password = password;
            ActionNumber = actionNumber;
        }

        [Required] public string User { get; set; }
        [Required] public int ItemId { get; set; }
        [Required] public int ShoppingListId { get; set; }
        [Required] public string Password { get; set; }
        [Required] public int ActionNumber { get; set; }
    }
}