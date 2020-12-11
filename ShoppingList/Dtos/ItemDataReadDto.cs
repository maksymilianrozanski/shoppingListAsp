using System.ComponentModel.DataAnnotations;
using ShoppingList.Entities;

namespace ShoppingList.Dtos
{
    public class ItemDataReadDto
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public int Quantity { get; set; }

        [Required] public string ItemType { get; set; }

        public static implicit operator ItemDataReadDto(ItemDataEntity itemDataEntity) =>
            new ItemDataReadDto
            {
                Id = itemDataEntity.Id,
                Name = itemDataEntity.Name,
                Quantity = itemDataEntity.Quantity,
                ItemType = itemDataEntity.ItemType
            };
    }
}