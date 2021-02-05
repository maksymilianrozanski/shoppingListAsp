using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharedTypes.Entities
{
    public class ShopWaypointsEntity
    {
        [Key] public int Id { get; set; }

        [Required, MaxLength(200)] public string Name { get; set; }

        [Required] public string ShopWaypointsReadDtoJson { get; set; }
        public ICollection<ShoppingListEntity> ShoppingListEntities { get; set; } = new List<ShoppingListEntity>();
    }
}