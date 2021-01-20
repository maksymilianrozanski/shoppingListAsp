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
            ShopName = ShopName,
            ItemDataEntities = new List<ItemDataEntity>
            {
                new()
                {
                    Id = 10,
                    Name = "milk",
                    Quantity = 1,
                    ItemType = "ToBuy"
                }
            }
        };

        [Test]
        public void ShouldReturnShoppingListEntityWhenContainsNonEmptyShopName()
        {
            Option<ShoppingListEntity> expected = _shoppingListEntity;
            var result = SqlShoppingListRepo.ShouldTryFindWaypoints(_shoppingListEntity);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnNoneWhenEntityContainsEmptyShopName()
        {
            var result = SqlShoppingListRepo.ShouldTryFindWaypoints(new ShoppingListEntity()
            {
                Id = 43,
                Password = "Password",
                ShopName = "",
                ItemDataEntities = new List<ItemDataEntity>()
            });

            Assert.AreEqual(new None(), result);
        }

        [Test]
        public void ShouldJoinWaypointsWithShoppingList()
        {
            var waypoints = new ShopWaypointsReadDto(ShopName,
                new Waypoint("start", 699, 673),
                new Waypoint("checkout", 469, 584),
                new List<Waypoint>
                {
                    new("DRINK_JUICE", 283, 295),
                    new("VEGETABLES", 607, 332),
                }
            );

            Either<ShopWaypointsNotFound, (ShopWaypointsReadDto, ShoppingListModule.ShoppingList)> expected =
                (waypoints, _shoppingListEntity);

            Option<ShopWaypointsReadDto> WaypointsFunc(string name) =>
                name == ShopName ? waypoints : new None();

            var result = SqlShoppingListRepo.MatchWaypointsToShoppingList(_shoppingListEntity, WaypointsFunc);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnWaypointsNotFound()
        {
            static Option<ShopWaypointsReadDto> WaypointsFunc(string name) => new();

            var result = SqlShoppingListRepo.MatchWaypointsToShoppingList(_shoppingListEntity, WaypointsFunc);

            result.Match(Assert.IsInstanceOf<ShopWaypointsNotFound>,
                _ => Assert.Fail("result should match to the left"));
        }

        [Test]
        public void ShouldApplyConversionFunctionAndReturnLeft()
        {
            var leftValue1 = "1234";
            var targetLeftValue = 1234;

            Either<int, double> expected = targetLeftValue;

            var result = SqlShoppingListRepo.FlattenErrors<string, int, double>
                (leftValue1, int.Parse);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnRight()
        {
            Either<int, double> expected = 1234.5;
            Either<string, Either<int, double>> input = (Either<int, double>) 1234.5;

            var result = SqlShoppingListRepo.FlattenErrors(input, int.Parse);

            Assert.AreEqual(expected, result);
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
                        ShoppingListErrors.ShoppingListErrors.ListItemNotFound));

            var result = SqlShoppingListRepo.ApplyItemDataAction2(GetById, itemDataAction);
            Assert.AreEqual(expected, result);
        }
    }
}