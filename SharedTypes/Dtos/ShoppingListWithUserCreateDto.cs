using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LaYumba.Functional;
using SharedTypes.Entities;

namespace SharedTypes.Dtos
{
    public class ShoppingListWithUserCreateDto : ShoppingListCreateDto
    {
        [Required] public int UserId { get; set; }

        public new ShoppingListEntity ToShoppingListEntity(Option<ShopWaypointsEntity> waypoints)
        {
            var entity = new ShoppingListEntity
            {
                Id = 0,
                // todo: refactor/remove password from ShoppingListEntity
                ItemDataEntities = new List<ItemDataEntity>(),
                UserEntityId = this.UserId
            };

            waypoints.ForEach(i => entity.ShopWaypointsEntity = i);
            return entity;
        }

        public ShoppingListWithUserCreateDto(int userId, string shopName)
        {
            UserId = userId;
            Password = "";
            ShopName = shopName;
        }
    }
}