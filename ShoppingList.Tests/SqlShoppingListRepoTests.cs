using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.FSharp.Core;
using NUnit.Framework;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using SharedTypes.Entities;
using ShoppingData;
using ShoppingList.Data.List;
using ShoppingList.Data.List.Errors;
using ShoppingList.Utils;
using static Waypoints.WaypointsModule;

namespace ShoppingListTests
{
    public class SqlShoppingListRepoTests
    {
        const string ShopName = "Grocery Store";

        private readonly ShoppingListEntity _shoppingListEntity = new()
        {
            Id = 42,
            Password = "password",
            ItemDataEntities = new List<ItemDataEntity>
            {
                new()
                {
                    Id = 10,
                    Name = "milk",
                    Quantity = 1,
                    ItemType = "ToBuy"
                }
            },
            ShopWaypointsEntityId = 8,
        };

        [Test]
        public void ShouldReturnShoppingListEntity_WhenContainsNonNullShopWaypointsEntityId()
        {
            Option<ShoppingListEntity> expected = _shoppingListEntity;
            var result = SqlShoppingListRepo.ShouldTryFindWaypoints(_shoppingListEntity);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnNone_WhenShopWaypointsEntityIdIsNull()
        {
            var result = SqlShoppingListRepo.ShouldTryFindWaypoints(new ShoppingListEntity()
            {
                Id = 43,
                Password = "Password",
                ItemDataEntities = new List<ItemDataEntity>(),
                ShopWaypointsEntityId =  null
            });

            Assert.AreEqual(new None(), result);
        }

        [Test]
        public void ShouldAddItemToShoppingList()
        {
            var createDto = new ItemDataCreateDto(42, "sugar", 1);

            Option<ShoppingListEntity> GetById(int i) => _shoppingListEntity;

            var shoppingListWithSugar = _shoppingListEntity.With(
                i => i.ItemDataEntities, new List<ItemDataEntity>
                {
                    _shoppingListEntity.ItemDataEntities.First(),
                    new()
                    {
                        Name = "sugar",
                        Quantity = 1,
                        ItemType = "ToBuy"
                    }
                });
            Option<(ShoppingListEntity shoppingListEntity, ShoppingListModule.ShoppingList result)> expected =
                (_shoppingListEntity, shoppingListWithSugar);

            var result = SqlShoppingListRepo.AddItemToList2(GetById, createDto);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnNoneWhenEntityNotFound()
        {
            var createDto = new ItemDataCreateDto(42, "sugar", 1);
            static Option<ShoppingListEntity> GetById(int i) => new None();

            var result = SqlShoppingListRepo.AddItemToList2(GetById, createDto);
            Assert.AreEqual(new None(), result);
        }

        [Test]
        public void ShouldReturnUpdatedShoppingList()
        {
            Option<ShoppingListEntity> GetById(int i) => _shoppingListEntity;
            var itemDataAction = new ItemDataActionDto("user",
                _shoppingListEntity.ItemDataEntities.First().Id,
                _shoppingListEntity.Id,
                ItemDataActionDto.ItemDataActions.ItemToBought);

            var updatedShoppingList = _shoppingListEntity.With(i => i.ItemDataEntities, new List<ItemDataEntity>
            {
                _shoppingListEntity.ItemDataEntities.First().With(i => i.ItemType, "Bought")
            });

            Option<(ShoppingListEntity entityFromDb,
                FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors> result)> expected =
                (_shoppingListEntity,
                    FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors>.NewChoice1Of2(
                        updatedShoppingList));

            var result = SqlShoppingListRepo.ApplyItemDataAction2(GetById, itemDataAction);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnNoneIfEntityNotFound()
        {
            Option<ShoppingListEntity> GetById(int i) => new None();
            var itemDataAction = new ItemDataActionDto("user",
                _shoppingListEntity.ItemDataEntities.First().Id,
                _shoppingListEntity.Id,
                ItemDataActionDto.ItemDataActions.ItemToBought);

            var result = SqlShoppingListRepo.ApplyItemDataAction2(GetById, itemDataAction);
            Assert.AreEqual(new None(), result);
        }

        [Test]
        public void ShouldReturnNoneIfInvalidActionNumber()
        {
            Option<ShoppingListEntity> GetById(int i) => new None();
            var itemDataAction = new ItemDataActionDto("user",
                _shoppingListEntity.ItemDataEntities.First().Id, _shoppingListEntity.Id,
                (ItemDataActionDto.ItemDataActions) 22222);

            var result = SqlShoppingListRepo.ApplyItemDataAction2(GetById, itemDataAction);
            Assert.AreEqual(new None(), result);
        }

        [Test]
        public void ShouldReturnChoice2Of2()
        {
            Option<ShoppingListEntity> GetById(int i) => _shoppingListEntity;
            var itemDataAction = new ItemDataActionDto("user",
                222222,
                _shoppingListEntity.Id,
                ItemDataActionDto.ItemDataActions.ItemToBought);

            Option<(ShoppingListEntity entityFromDb,
                FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors> result)> expected =
                (_shoppingListEntity,
                    FSharpChoice<ShoppingListModule.ShoppingList, ShoppingListErrors.ShoppingListErrors>.NewChoice2Of2(
                        ShoppingListErrors.ShoppingListErrors.NotFound));

            var result = SqlShoppingListRepo.ApplyItemDataAction2(GetById, itemDataAction);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnRightShoppingListCreateDtoWithWaypointsId()
        {
            static Option<int> GetWaypointsId(string name) => 22;

            var createDto = new ShoppingListCreateDto("password123", "valid-shop");

            Either<ShoppingListErrors.ShoppingListErrors,
                (ShoppingListCreateDto, Option<int>)> expected = (createDto, 22);

            var result = SqlShoppingListRepo.GetWaypointsByName(GetWaypointsId, createDto);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnRightShoppingListCreateDtoWithoutWaypointsId_WhenEmptyShopName()
        {
            static Option<int> GetWaypointsId(string name)
            {
                Assert.Fail("should not have called this function, when empty ShopName");
                throw new Exception();
            }

            var createDto = new ShoppingListCreateDto("password123", "");

            Either<ShoppingListErrors.ShoppingListErrors,
                (ShoppingListCreateDto, Option<int>)> expected = (createDto, new None());

            var result = SqlShoppingListRepo.GetWaypointsByName(GetWaypointsId, createDto);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnLeft_WhenShopNotFound()
        {
            static Option<int> GetWaypointsId(string name) => new None();

            var createDto = new ShoppingListCreateDto("password123", "valid-shop");

            Either<ShoppingListErrors.ShoppingListErrors,
                (ShoppingListCreateDto, Option<int>)> expected = ShoppingListErrors.ShoppingListErrors.ShopNotFound;

            var result = SqlShoppingListRepo.GetWaypointsByName(GetWaypointsId, createDto);
            Assert.AreEqual(expected, result);
        }
    }
}