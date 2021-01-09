using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using ShoppingData;
using static SharedTypes.Dtos.ItemDataActionDto;
using ModifyItemDataAction =
    Microsoft.FSharp.Core.FSharpFunc<string, Microsoft.FSharp.Core.FSharpFunc<int, Microsoft.FSharp.Core.FSharpFunc<
        ShoppingData.ShoppingListModule.ShoppingList, Microsoft.FSharp.Core.FSharpChoice<
            ShoppingData.ShoppingListModule.ShoppingList, ShoppingData.ShoppingListErrors.ShoppingListErrors>>>>;

namespace SharedTypes.Dtos.Protected
{
    public class ItemDataActionDtoNoPassword
    {
        public static readonly ImmutableDictionary<ItemDataActions, ModifyItemDataAction> Actions =
            new Dictionary<ItemDataActions, ModifyItemDataAction>
            {
                {0, ShoppingListNoPassModule.listItemToAssigned},
                {(ItemDataActions) 1, ShoppingListNoPassModule.listItemToNotFound},
                {(ItemDataActions) 2, ShoppingListNoPassModule.listItemToBought},
                {(ItemDataActions) 3, ShoppingListNoPassModule.listItemToCancelled}
            }.ToImmutableDictionary();

        public ItemDataActionDtoNoPassword(string user, int itemId, int shoppingListId, int actionNumber)
        {
            User = user;
            ItemId = itemId;
            ShoppingListId = shoppingListId;
            ActionNumber = actionNumber;
        }

        [Required] public string User { get; set; }
        [Required] public int ItemId { get; set; }
        [Required] public int ShoppingListId { get; set; }
        [Required] public int ActionNumber { get; set; }
    }
}