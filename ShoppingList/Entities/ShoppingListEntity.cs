using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FSharpPlus;
using LaYumba.Functional;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.FSharp.Collections;
using ShoppingData;
using ShoppingList.Dtos;

namespace ShoppingList.Entities
{
    public class ShoppingListEntity
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Password { get; set; }

        [Required] public ICollection<ItemDataEntity> ItemDataEntities { get; set; }

        public static implicit operator ShoppingListEntity(ShoppingListCreateDto createDto) =>
            new ShoppingListEntity
            {
                Id = 0,
                Name = createDto.Name,
                Password = createDto.Password,
                ItemDataEntities = new List<ItemDataEntity>()
            };

        public static implicit operator ShoppingListModule.ShoppingList(ShoppingListEntity entity) =>
            new ShoppingListModule.ShoppingList(entity.Id, entity.Name, entity.Password,
                ListModule.OfSeq(
                    entity.ItemDataEntities.Map(i =>
                        (ShoppingItemModule.ItemData) i))
            );

        public static implicit operator ShoppingListEntity(ShoppingListModule.ShoppingList list) =>
            new ShoppingListEntity
            {
                Id = list.Id,
                Name = list.Name,
                Password = list.Password,
                ItemDataEntities = list.Items.Map(i => (ItemDataEntity) i).ToList()
            };

        public ShoppingListEntity Merge(ShoppingListUpdateDto changed)
        {
            this.Name = changed.Name;
            this.Password = changed.Password;
            this.ItemDataEntities =
                changed.Items.Map(i => i.ToItemDataEntity(this.Id, this)).ToList();
            return this;
        }
    }
}