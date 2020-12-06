using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using ShoppingData;

namespace ShoppingList.Dtos
{
    public class ShoppingListCreateDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public List<ShoppingItemModule.ItemData> Items { get; set; }
    }
}