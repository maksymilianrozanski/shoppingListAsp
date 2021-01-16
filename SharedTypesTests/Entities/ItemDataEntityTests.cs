using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using SharedTypes.Entities;
using ShoppingData;
using static ShoppingData.ShoppingItemModule.ItemType;


namespace SharedTypesTests.Entities
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
                new(10, "sugar", 1, Bought),
                new(11, "coffee", 20, LookingFor),
                new(12, "milk", 21, ToBuy),
                new(13, "apples", 22, Cancelled),
                new(14, "pineapple", 23, NotFound),
            };

            var expectedItems = new List<ItemDataEntity>
            {
                new() {Id = 10, Name = "sugar", Quantity = 1, ItemType = "Bought"},
                new() {Id = 11, Name = "coffee", Quantity = 20, ItemType = "LookingFor"},
                new() {Id = 12, Name = "milk", Quantity = 21, ItemType = "ToBuy"},
                new() {Id = 13, Name = "apples", Quantity = 22, ItemType = "Cancelled"},
                new() {Id = 14, Name = "pineapple", Quantity = 23, ItemType = "NotFound"}
            };
            Assert.True(AreAllEqual(expectedItems, itemsToConvert));
        }

        [Test]
        public void ConvertedItemShouldNotBeEqual()
        {
            var item = new ShoppingItemModule.ItemData(11, "coffee", 20,
                LookingFor);

            var notEqual = new ShoppingItemModule.ItemData(11, "coffee", 20, ToBuy);
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

        [Test]
        public void ShouldConvertStringToItemType()
        {
            var items = new List<string>
                {"ToBuy", "Bought", "Cancelled", "NotFound", "LookingFor"};
            var expected = new List<ShoppingItemModule.ItemType>
                {ToBuy, Bought, Cancelled, NotFound, LookingFor};

            var results = items.Map(ItemDataEntity.ItemTypeFromString).ToList();
            expected.Zip(results).ForEach(i => Assert.AreEqual(i.First, i.Second));
            Assert.AreEqual(expected.Count, results.Count);
        }

        [Test]
        public void ShouldConvertItemDataEntityToItemData()
        {
            var item = new ItemDataEntity
            {
                Id = 21,
                Name = "Milk",
                Quantity = 3,
                ItemType = "LookingFor",
                ShoppingListEntityRefId = 10
            };

            var expected = new ShoppingItemModule.ItemData(21, "Milk", 3, LookingFor);
            Assert.True(expected.Equals(item));
        }
    }
}