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
        [Key] public int Id { get; set; }

        [Required] public string Password { get; set; } = "";

        [Required] public string ShopName { get; set; } = "";

        [Required] public ICollection<ItemDataEntity> ItemDataEntities { get; set; } = new List<ItemDataEntity>();

        public static implicit operator ShoppingListEntity(ShoppingListCreateDto createDto) =>
            new()
            {
                Id = 0,
                Password = createDto.Password,
                ShopName = createDto.ShopName,
                ItemDataEntities = new List<ItemDataEntity>()
            };

        public static implicit operator ShoppingListModule.ShoppingList(ShoppingListEntity entity) =>
            new(entity.Id, entity.Password, entity.ShopName,
                ListModule.OfSeq(
                    entity.ItemDataEntities.Map(i =>
                        (ShoppingItemModule.ItemData) i))
            );

        public static implicit operator ShoppingListEntity(ShoppingListModule.ShoppingList list) =>
            new()
            {
                Id = list.Id,
                Password = list.Password,
                ItemDataEntities = list.Items.Map(i => (ItemDataEntity) i).ToList()
            };
    }
}