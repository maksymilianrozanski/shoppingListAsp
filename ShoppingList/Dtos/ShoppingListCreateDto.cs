using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.FSharp.Collections;
using ShoppingData;

namespace ShoppingList.Dtos
{
    public class ShoppingListCreateDto
    {
        [Required] [MaxLength(100)] public string Name { get; set; }
        [Required] [MaxLength(20)] public string Password { get; set; }
    }
}