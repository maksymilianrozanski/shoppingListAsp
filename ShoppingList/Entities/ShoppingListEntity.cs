using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required] public IEnumerable<ItemDataEntity> Items { get; set; }

        public static implicit operator ShoppingListEntity(ShoppingListCreateDto createDto) =>
            new ShoppingListEntity
            {
                Id = 0,
                Name = createDto.Name,
                Password = createDto.Password,
                Items = new List<ItemDataEntity>()
            };
    }
}