using System.ComponentModel.DataAnnotations;
using ShoppingList.Entities;

namespace ShoppingList.Dtos
{
    public class ItemDataUpdateDto
    {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public int Quantity { get; set; }

        [Required] public string ItemType { get; set; }

        public ItemDataEntity ToItemDataEntity(int parentId, ShoppingListEntity parent)
        {
            return new ItemDataEntity
            {
                Id = this.Id,
                Name = this.Name,
                Quantity = this.Quantity,
                ItemType = this.ItemType,
                ShoppingListEntityRefId = parentId,
                ShoppingListEntity = parent
            };
        }
    }
}