using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LaYumba.Functional;
using SharedTypes.Entities;

namespace SharedTypes.Dtos
{
    public class ShoppingListCreateDto
    {
        public ShoppingListCreateDto(string password, string shopName)
        {
            Password = password;
            ShopName = shopName;
        }

        public ShoppingListCreateDto()
        {
            Password = "";
            ShopName = "";
        }

        [Required] [MaxLength(20)] public string Password { get; set; }

        [MaxLength(100)]
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ShopName { get; set; }

        public ShoppingListEntity ToShoppingListEntity(Option<ShopWaypointsEntity> waypoints)
        {
            var entity = new ShoppingListEntity
            {
                Id = 0,
                ItemDataEntities = new List<ItemDataEntity>()
            };

            waypoints.ForEach(i => entity.ShopWaypointsEntity = i);
            return entity;
        }
    }
}