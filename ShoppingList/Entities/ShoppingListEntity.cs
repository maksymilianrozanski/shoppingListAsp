using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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