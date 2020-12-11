using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Microsoft.FSharp.Core;
using ShoppingData;

namespace ShoppingList.Entities
{
    public class ItemDataEntity
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public int Quantity { get; set; }

        [Required] public string ItemType { get; set; }

        [Required] public int ShoppingListEntityRefId { get; set; }
        public ShoppingListEntity ShoppingListEntity { get; set; }

        public static implicit operator ItemDataEntity(ShoppingItemModule.ItemData itemData) =>
            new ItemDataEntity
            {
                Id = itemData.Id,
                Name = itemData.Name,
                Quantity = itemData.Quantity,
                ItemType = ItemTypeToString(itemData.ItemType)
            };

        private static string ItemTypeToString(ShoppingItemModule.ItemType itemType)
        {
            return itemType switch
            {
                ShoppingItemModule.ItemType.Assigned assigned => "Assigned " + assigned.Item,
                var x when x.IsToBuy => "ToBuy",
                var x when x.IsBought => "Bought",
                var x when x.IsCancelled => "Cancelled",
                var x when x.IsNotFound => "NotFound",
                _ => throw new MatchFailureException()
            };
        }
    }
}