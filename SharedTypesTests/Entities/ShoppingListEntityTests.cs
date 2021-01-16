using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using NUnit.Framework;
using SharedTypes.Entities;
using ShoppingData;

namespace SharedTypesTests.Entities
{
    public class ShoppingListEntityTests
    {
        [Test]
        public void ShouldConvertShoppingListEntityToShoppingList()
        {
            var shoppingListEntity = new ShoppingListEntity
            {
                Id = 10,
                Password = "pass",
                ShopName = "shopName",
                ItemDataEntities = new List<ItemDataEntity>()
            };

            var item1 = new ItemDataEntity
            {
                Id = 20,
                Name = "Coffee",
                Quantity = 2,
                ItemType = "ToBuy",
                ShoppingListEntityRefId = 10,
                ShoppingListEntity = shoppingListEntity
            };

            var item2 = new ItemDataEntity
            {
                Id = 21,
                Name = "Milk",
                Quantity = 3,
                ItemType = "LookingFor",
                ShoppingListEntityRefId = 10,
                ShoppingListEntity = shoppingListEntity
            };

            shoppingListEntity.ItemDataEntities.Add(item1);
            shoppingListEntity.ItemDataEntities.Add(item2);
            ICollection<ShoppingItemModule.ItemData> convertedItemData = new List<ShoppingItemModule.ItemData>
            {
                new ItemDataEntity
                {
                    Id = 20,
                    Name = "Coffee",
                    Quantity = 2,
                    ItemType = "ToBuy",
                    ShoppingListEntityRefId = 10,
                    ShoppingListEntity = shoppingListEntity
                },
                new ItemDataEntity
                {
                    Id = 21,
                    Name = "Milk",
                    Quantity = 3,
                    ItemType = "LookingFor",
                    ShoppingListEntityRefId = 10,
                    ShoppingListEntity = shoppingListEntity
                }
            };
            var expected = new ShoppingListModule.ShoppingList(10, "pass", "shopName",
                ListModule.OfSeq(convertedItemData));

            Assert.AreEqual(expected, (ShoppingListModule.ShoppingList) shoppingListEntity);
            Assert.True(expected.Equals(shoppingListEntity));
            Assert.True(((ShoppingListModule.ShoppingList) shoppingListEntity).Equals(expected));
        }
    }
}