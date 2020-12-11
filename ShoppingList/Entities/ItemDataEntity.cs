using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Microsoft.FSharp.Core;
using ShoppingData;
using ShoppingList.Dtos;

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

        public static ShoppingItemModule.ItemType ItemTypeFromString(string itemType) =>
            itemType switch
            {
                "ToBuy" => ShoppingItemModule.ItemType.ToBuy,
                "Bought" => ShoppingItemModule.ItemType.Bought,
                "Cancelled" => ShoppingItemModule.ItemType.Cancelled,
                "NotFound" => ShoppingItemModule.ItemType.NotFound,
                var x when x.StartsWith("Assigned ") && x.Length > 9 =>
                    ShoppingItemModule.ItemType.NewAssigned(x.Substring(9)),
                _ => throw new MatchFailureException()
            };

        public static implicit operator ItemDataEntity(ItemDataCreateDto itemData) =>
            new ItemDataEntity
            {
                Id = 0,
                Name = itemData.Name,
                Quantity = itemData.Quantity,
                ItemType = itemData.ItemType,
                ShoppingListEntityRefId = itemData.ShoppingListId
            };

        public static implicit operator ShoppingItemModule.ItemData(ItemDataEntity entity) =>
            new ShoppingItemModule.ItemData(entity.Id, entity.Name, entity.Quantity,
                ItemTypeFromString(entity.ItemType));
    }
}