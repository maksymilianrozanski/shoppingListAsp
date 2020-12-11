using System.Collections.Generic;
using Microsoft.FSharp.Collections;
using NUnit.Framework;
using ShoppingData;
using ShoppingList.Entities;

namespace ShoppingListTests.Entities
{
    public class ShoppingListEntityTests
    {
        [Test]
        public void ShouldConvertShoppingListEntityToShoppingList()
        {
            var shoppingListEntity = new ShoppingListEntity
            {
                Id = 10,
                Name = "Some list",
                Password = "pass",
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
                ItemType = "Assigned somebody",
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
                    ItemType = "Assigned somebody",
                    ShoppingListEntityRefId = 10,
                    ShoppingListEntity = shoppingListEntity
                }
            };
            var expected = new ShoppingListModule.ShoppingList(10, "Some list", "pass",
                ListModule.OfSeq(convertedItemData));

            Assert.AreEqual(expected, (ShoppingListModule.ShoppingList) shoppingListEntity);
            Assert.True(expected.Equals(shoppingListEntity));
            Assert.True(((ShoppingListModule.ShoppingList) shoppingListEntity).Equals(expected));
        }

        [Test]
        public void ComparedItemsShoppingListsShouldNotBeEqual()
        {
            var shoppingListEntity = new ShoppingListEntity
            {
                Id = 10,
                Name = "Some list",
                Password = "pass",
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
                ItemType = "Assigned somebody",
                ShoppingListEntityRefId = 10,
                ShoppingListEntity = shoppingListEntity
            };

            shoppingListEntity.ItemDataEntities.Add(item1);
            shoppingListEntity.ItemDataEntities.Add(item2);

            ICollection<ShoppingItemModule.ItemData> otherItems = new List<ShoppingItemModule.ItemData>
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
                    ItemType = "Assigned SOMEBODY ELSE",
                    ShoppingListEntityRefId = 10,
                    ShoppingListEntity = shoppingListEntity
                }
            };

            var withOtherItem = new ShoppingListModule.ShoppingList(10, "Some list", "pass",
                ListModule.OfSeq(otherItems));

            Assert.AreNotEqual(withOtherItem, (ShoppingListModule.ShoppingList) shoppingListEntity);
            Assert.False(withOtherItem.Equals(shoppingListEntity));
            Assert.False(((ShoppingListModule.ShoppingList) shoppingListEntity).Equals(withOtherItem));
        }
    }
}