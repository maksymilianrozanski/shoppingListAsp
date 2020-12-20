using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Microsoft.FSharp.Core;
using ShoppingData;

namespace ShoppingList.Dtos
{
    using ModifyItemDataAction = FSharpFunc<string, FSharpFunc<int, FSharpFunc<
        ShoppingListModule.ShoppingList, FSharpFunc<string,
            FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors>>>>>;

    public class ItemDataActionDto
    {
        [Required] public string User { get; set; }
        [Required] public int ItemId { get; set; }
        [Required] public int ShoppingListId { get; set; }
        [Required] public string Password { get; set; }
        [Required] public int ActionNumber { get; set; }

        public static readonly ImmutableDictionary<ItemDataActions, ModifyItemDataAction> Actions =
            new Dictionary<ItemDataActions, ModifyItemDataAction>
            {
                {(ItemDataActions) 0, ShoppingListModule.listItemToAssigned},
                {(ItemDataActions) 1, ShoppingListModule.listItemToNotFound},
                {(ItemDataActions) 2, ShoppingListModule.listItemToBought},
                {(ItemDataActions) 3, ShoppingListModule.listItemToCancelled}
            }.ToImmutableDictionary();

        public ItemDataActionDto(string user, int itemId, int shoppingListId, string password, int actionNumber)
        {
            User = user;
            ItemId = itemId;
            ShoppingListId = shoppingListId;
            Password = password;
            ActionNumber = actionNumber;
        }
        
        public enum ItemDataActions
        {
            AssignItem = 0,
            ItemToNotFound = 1,
            ItemToBought = 2,
            ItemToCancelled = 3
        }
    }
}