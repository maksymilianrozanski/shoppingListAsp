using System;
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
            var itemData = new ShoppingItemModule.ItemData(10, "sugar", 12, ShoppingItemModule.ItemType.Bought);
            var expected = new ItemDataEntity
            {
                Id = 10, Name = "sugar", Quantity = 12, ItemType = "Bought"
            };

            var result = itemData;

            AreEqual(expected, result);
        }

        [Test]
        public void ShouldConvertItemDataToItemDataEntity2()
        {
            var itemData =
                new ShoppingItemModule.ItemData(10, "sugar", 12, ShoppingItemModule.ItemType.NewAssigned("Garfield"));
            var expected = new ItemDataEntity
            {
                Id = 10, Name = "sugar", Quantity = 12, ItemType = "Assigned Garfield"
            };

            var result = itemData;

            AreEqual(expected, result);
        }

        private static void AreEqual(ItemDataEntity expected, ItemDataEntity result)
        {
            Assert.AreEqual(expected.Id, result.Id, "Id values should be equal");
            Assert.AreEqual(expected.Name, result.Name, "Name values should be equal");
            Assert.AreEqual(expected.Quantity, result.Quantity, "Quantity values should be equal");
            Assert.AreEqual(expected.ItemType, result.ItemType, "ItemType values should be equal");
        }
    }
}