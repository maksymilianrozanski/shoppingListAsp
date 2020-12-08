using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.FSharp.Collections;
using ShoppingData;

namespace ShoppingList.Entities
{
    public class ShoppingListEntity
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Password { get; set; }

        [Required] public IEnumerable<ItemDataEntity> Items { get; set; }
    }
}