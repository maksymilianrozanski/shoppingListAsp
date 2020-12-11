using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShoppingList.Entities;

namespace ShoppingList.Dtos
{
    public class ShoppingListUpdateDto
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Password { get; set; }

        [Required] public ICollection<ItemDataUpdateDto> Items { get; set; }
    }
}