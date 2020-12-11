using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using ShoppingData;
using ShoppingList.Entities;
using static ShoppingData.ShoppingItemModule.ItemType;


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
                new ShoppingItemModule.ItemData(10, "sugar", 1, Bought),
                new ShoppingItemModule.ItemData(11, "coffee", 20, NewAssigned("Garfield")),
                new ShoppingItemModule.ItemData(12, "milk", 21, ToBuy),
                new ShoppingItemModule.ItemData(13, "apples", 22, Cancelled),
                new ShoppingItemModule.ItemData(14, "pineapple", 23, NotFound),
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
                NewAssigned("Garfield"));

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
                {"ToBuy", "Bought", "Cancelled", "NotFound", "Assigned Garfield", "Assigned Some Cat"};
            var expected = new List<ShoppingItemModule.ItemType>
                {ToBuy, Bought, Cancelled, NotFound, NewAssigned("Garfield"), NewAssigned("Some Cat")};

            var results = items.Map(ItemDataEntity.ItemTypeFromString).ToList();

            expected.Zip(results).ForEach(i => Assert.AreEqual(i.First, i.Second));
            Assert.AreEqual(expected.Count, results.Count);
        }

        [Test]
        public void ShouldThrowMatchFailureException()
        {
            var invalidItems = new List<string> {"Cat", "AssignedCat", "", "Assigned", "Assigned "};
            invalidItems.ForEach(i =>
                    Assert.Throws<MatchFailureException>(
                        () => ItemDataEntity.ItemTypeFromString(i)));
        }

        [Test]
        public void ShouldConvertItemDataEntityToItemData()
        {
            var item = new ItemDataEntity
            {
                Id = 21,
                Name = "Milk",
                Quantity = 3,
                ItemType = "Assigned somebody",
                ShoppingListEntityRefId = 10
            };

            var expected = new ShoppingItemModule.ItemData(21, "Milk", 3, NewAssigned("somebody"));
            Assert.True(expected.Equals(item));
        }
    }
}