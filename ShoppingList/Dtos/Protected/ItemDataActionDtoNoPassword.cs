using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Microsoft.FSharp.Core;
using ShoppingData;
using static ShoppingList.Dtos.ItemDataActionDto;
using ModifyItemDataAction =
    Microsoft.FSharp.Core.FSharpFunc<string, Microsoft.FSharp.Core.FSharpFunc<int, Microsoft.FSharp.Core.FSharpFunc<
        ShoppingData.ShoppingListModule.ShoppingList, Microsoft.FSharp.Core.FSharpChoice<
            ShoppingData.ShoppingListModule.ShoppingList, ShoppingData.ShoppingListErrors.ShoppingListErrors>>>>;

namespace ShoppingList.Dtos.Protected
{
    public class ItemDataActionDtoNoPassword
    {
        [Required] public string User { get; set; }
        [Required] public int ItemId { get; set; }
        [Required] public int ShoppingListId { get; set; }
        [Required] public int ActionNumber { get; set; }

        public static readonly ImmutableDictionary<ItemDataActions, ModifyItemDataAction> Actions =
            new Dictionary<ItemDataActions, ModifyItemDataAction>
            {
                {(ItemDataActions) 0, ShoppingListNoPassModule.listItemToAssigned},
                {(ItemDataActions) 1, ShoppingListNoPassModule.listItemToNotFound},
                {(ItemDataActions) 2, ShoppingListNoPassModule.listItemToBought},
                {(ItemDataActions) 3, ShoppingListNoPassModule.listItemToCancelled},
            }.ToImmutableDictionary();

        public ItemDataActionDtoNoPassword(string user, int itemId, int shoppingListId, int actionNumber)
        {
            User = user;
            ItemId = itemId;
            ShoppingListId = shoppingListId;
            ActionNumber = actionNumber;
        }

        public static ItemDataActionDtoNoPassword FromItemDataReadDto(ItemDataReadDto readDto,
            ItemDataActions itemDataAction, int shoppingListId) =>
            new(readDto.Name, readDto.Id, shoppingListId, (int) itemDataAction);
    }
}