using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LaYumba.Functional;
using Microsoft.FSharp.Collections;
using SharedTypes.Dtos;
using ShoppingData;

namespace SharedTypes.Entities
{
    public class ShoppingListEntity
    {
        public int? ShopWaypointsEntityId { get; set; }
        [Key] public int Id { get; set; }

        public string Password { get;  } = "";

        [Required] public int UserEntityId { get; set; }

        public UserEntity UserEntity { get; set; }

        [Required] public ICollection<ItemDataEntity> ItemDataEntities { get; set; } = new List<ItemDataEntity>();
        public ShopWaypointsEntity? ShopWaypointsEntity { get; set; }

        public static implicit operator ShoppingListModule.ShoppingList(ShoppingListEntity entity) =>
            new(entity.Id, entity.Password,
                ListModule.OfSeq(
                    entity.ItemDataEntities.Map(i =>
                        (ShoppingItemModule.ItemData) i))
            );

        public static implicit operator ShoppingListEntity(ShoppingListModule.ShoppingList list) =>
            new()
            {
                Id = list.Id,
                ItemDataEntities = list.Items.Map(i => (ItemDataEntity) i).ToList()
            };
    }
}