using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LaYumba.Functional;
using SharedTypes.Entities;

namespace SharedTypes.Dtos
{
    public class ShoppingListCreateDto
    {
        public ShoppingListCreateDto(string shopName)
        {
            ShopName = shopName;
        }

        public ShoppingListCreateDto(int userId, string shopName)
        {
            ShopName = shopName;
            UserId = userId;
        }

        [MaxLength(100)]
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ShopName { get; set; }

        [Required] public int UserId { get; set; }

        public ShoppingListEntity ToShoppingListEntity(Option<ShopWaypointsEntity> waypoints)
        {
            var entity = new ShoppingListEntity
            {
                Id = 0,
                ItemDataEntities = new List<ItemDataEntity>(),
                UserEntityId = this.UserId
            };

            waypoints.ForEach(i => entity.ShopWaypointsEntity = i);
            return entity;
        }
    }
}