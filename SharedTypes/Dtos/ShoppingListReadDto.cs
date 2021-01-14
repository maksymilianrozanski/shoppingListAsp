using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LaYumba.Functional;
using SharedTypes.Entities;

namespace SharedTypes.Dtos
{
    public class ShoppingListReadDto
    {
        [Key] public int Id { get; set; }

        [Required] public IEnumerable<ItemDataReadDto> Items { get; set; } = new List<ItemDataReadDto>();

        public static implicit operator ShoppingListReadDto(ShoppingListEntity entity) =>
            new()
            {
                Id = entity.Id,
                Items = entity.ItemDataEntities.Map(i => (ItemDataReadDto) i)
            };
    }
}