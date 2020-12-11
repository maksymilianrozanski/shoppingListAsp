using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LaYumba.Functional;
using ShoppingData;
using ShoppingList.Entities;

namespace ShoppingList.Dtos
{
    public class ShoppingListReadDto
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Password { get; set; }

        [Required] public IEnumerable<ItemDataReadDto> Items { get; set; }

        public static implicit operator ShoppingListReadDto(ShoppingListEntity entity) =>
            new ShoppingListReadDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Password = entity.Password,
                Items = entity.ItemDataEntities.Map(i => (ItemDataReadDto) i)
            };
    }
}