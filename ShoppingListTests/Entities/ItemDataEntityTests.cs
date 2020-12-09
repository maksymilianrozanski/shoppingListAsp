using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ShoppingData;
using ShoppingList.Entities;

namespace ShoppingListTests.Entities
{
    public class ItemDataEntityTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ShouldConvertItemDataToItemDataEntity()
        {
            var itemsToConvert = new List<ShoppingItemModule.ItemData>
            {
                new ShoppingItemModule.ItemData(10, "sugar", 1, ShoppingItemModule.ItemType.Bought),
                new ShoppingItemModule.ItemData(11, "coffee", 20, ShoppingItemModule.ItemType.NewAssigned("Garfield")),
                new ShoppingItemModule.ItemData(12, "milk", 21, ShoppingItemModule.ItemType.ToBuy),
                new ShoppingItemModule.ItemData(13, "apples", 22, ShoppingItemModule.ItemType.Cancelled),
                new ShoppingItemModule.ItemData(14, "pineapple", 23, ShoppingItemModule.ItemType.NotFound),
            };

            var expectedItems = new List<ItemDataEntity>
            {
                new ItemDataEntity {Id = 10, Name = "sugar", Quantity = 1, ItemType = "Bought"},
                new ItemDataEntity {Id = 11, Name = "coffee", Quantity = 20, ItemType = "Assigned Garfield"},
                new ItemDataEntity {Id = 12, Name = "milk", Quantity = 21, ItemType = "ToBuy"},
                new ItemDataEntity {Id = 13, Name = "apples", Quantity = 22, ItemType = "Cancelled"},
                new ItemDataEntity {Id = 14, Name = "pineapple", Quantity = 23, ItemType = "NotFound"}
            };

            Assert.True(AreAllEqual(expectedItems, itemsToConvert));
        }

        [Test]
        public void ConvertedItemShouldNotBeEqual()
        {
            var item = new ShoppingItemModule.ItemData(11, "coffee", 20,
                ShoppingItemModule.ItemType.NewAssigned("Garfield"));

            var notEqual = new ShoppingItemModule.ItemData(11, "coffee", 20, ShoppingItemModule.ItemType.ToBuy);
            Assert.AreNotEqual(item, notEqual);
        }

        private static bool AreAllEqual(IEnumerable<ItemDataEntity> expectedItems,
            IEnumerable<ShoppingItemModule.ItemData> actualItems) =>
            expectedItems.Zip(actualItems)
                .Select(i => EntitiesEqual(i))
                .All(i => i);

        private static bool EntitiesEqual((ItemDataEntity, ItemDataEntity) entities)
        {
            var (expected, actual) = entities;
            return expected.Id == actual.Id &&
                   expected.Name == actual.Name &&
                   expected.Quantity == actual.Quantity &&
                   expected.ItemType == actual.ItemType;
        }
    }
}