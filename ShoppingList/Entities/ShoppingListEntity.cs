using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using LaYumba.Functional;
using Microsoft.FSharp.Collections;
using ShoppingData;
using ShoppingList.Dtos;

namespace ShoppingList.Entities
{
    public class ShoppingListEntity
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; } = "";

        [Required] public string Password { get; set; } = "";

        [Required] public string ShopName { get; set; } = "";

        [Required] public ICollection<ItemDataEntity> ItemDataEntities { get; set; } = new List<ItemDataEntity>();

        public static implicit operator ShoppingListEntity(ShoppingListCreateDto createDto) =>
            new()
            {
                Id = 0,
                Name = createDto.Name,
                Password = createDto.Password,
                ShopName = createDto.ShopName,
                ItemDataEntities = new List<ItemDataEntity>()
            };

        public static implicit operator ShoppingListModule.ShoppingList(ShoppingListEntity entity) =>
            new(entity.Id, entity.Name, entity.Password, entity.ShopName,
                ListModule.OfSeq(
                    entity.ItemDataEntities.Map(i =>
                        (ShoppingItemModule.ItemData) i))
            );

        public static implicit operator ShoppingListEntity(ShoppingListModule.ShoppingList list) =>
            new()
            {
                Id = list.Id,
                Name = list.Name,
                Password = list.Password,
                ItemDataEntities = list.Items.Map(i => (ItemDataEntity) i).ToList()
            };
    }
}