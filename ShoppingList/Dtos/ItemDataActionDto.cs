using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Microsoft.FSharp.Core;
using ShoppingData;

namespace ShoppingList.Dtos
{
    using ItemDataAction =
        Func<string, int, ShoppingListModule.ShoppingList, string,
            FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors>>;

    public class ItemDataActionDto
    {
        [Required] public string User { get; set; }
        [Required] public int ItemId { get; set; }
        [Required] public int ShoppingListId { get; set; }
        [Required] public string Password { get; set; }
        [Required] public int ActionNumber { get; set; }
        
        public static ImmutableDictionary<ItemDataActions, FSharpFunc<string, FSharpFunc<int,
            FSharpFunc<ShoppingListModule.ShoppingList, FSharpFunc<string,
                FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors>>>>>> Actions =
            new Dictionary<ItemDataActions, FSharpFunc<string, FSharpFunc<int, FSharpFunc<
                ShoppingListModule.ShoppingList, FSharpFunc<string,
                    FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors>>>>>>
            {
                {(ItemDataActions) 0, ShoppingListModule.listItemToAssigned},
                {(ItemDataActions) 1, ShoppingListModule.listItemToNotFound},
                {(ItemDataActions) 2, ShoppingListModule.listItemToBought},
                {(ItemDataActions) 3, ShoppingListModule.listItemToCancelled}
            }.ToImmutableDictionary();

        public enum ItemDataActions
        {
            AssignItem = 0,
            ItemToNotFound = 1,
            ItemToBought = 2,
            ItemToCancelled = 3
        }
    }
}