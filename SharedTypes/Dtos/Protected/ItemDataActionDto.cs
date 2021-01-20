using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using ShoppingData;
using ModifyItemDataAction = Microsoft.FSharp.Core.FSharpFunc<string, Microsoft.FSharp.Core.FSharpFunc<int, Microsoft.FSharp.Core.FSharpFunc<
        ShoppingData.ShoppingListModule.ShoppingList, Microsoft.FSharp.Core.FSharpChoice<
            ShoppingData.ShoppingListModule.ShoppingList, ShoppingData.ShoppingListErrors.ShoppingListErrors>>>>;

namespace SharedTypes.Dtos.Protected
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
        
        public static readonly ImmutableDictionary<ItemDataActions, ModifyItemDataAction> Actions =
            new Dictionary<ItemDataActions, ModifyItemDataAction>
            {
                {0, ShoppingListModifyModule.listItemToLookingFor},
                {(ItemDataActions) 1, ShoppingListModifyModule.listItemToNotFound},
                {(ItemDataActions) 2, ShoppingListModifyModule.listItemToBought},
                {(ItemDataActions) 3, ShoppingListModifyModule.listItemToCancelled}
            }.ToImmutableDictionary();

        public ItemDataActionDto(string user, int itemId, int shoppingListId, ItemDataActions actionNumber)
        {
            User = user;
            ItemId = itemId;
            ShoppingListId = shoppingListId;
            ActionNumber = (int) actionNumber;
        }

        [Required] public string User { get; set; }
        [Required] public int ItemId { get; set; }
        [Required] public int ShoppingListId { get; set; }
        [Required] public int ActionNumber { get; set; }
    }
}