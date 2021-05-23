using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GroceryClassification;
using LaYumba.Functional;
using Microsoft.Extensions.ML;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using SharedTypes.Entities;
using ShoppingList.Auth;
using ShoppingList.Data.Waypoints;

namespace ShoppingList.Data.List
{
    public class SqlShoppingListRepoExampleData : SqlShoppingListRepo
    {
        public SqlShoppingListRepoExampleData(ShoppingListDbContext context,
            PredictionEnginePool<GroceryData, GroceryItemPrediction> predictionEnginePool,
            IUserPasswordSalt salt) : base(context,
            predictionEnginePool, salt)
        {
            if (context.ShopWaypointsEntities.ToList().Count == 0)
            {
                var userCreateDto = new UserCreateDto("user", "password");
                var userEntity = ToUserEntity(userCreateDto);

                context.UserEntities.Add(userEntity);

                SaveChanges();

                context.ShopWaypointsEntities.Add(new ShopWaypointsEntity
                {
                    Id = 0,
                    Name = "big-market",
                    ShoppingListEntities = new List<ShoppingListEntity>(),
                    ShopWaypointsReadDtoJson = JsonSerializer.Serialize(WaypointsRepoHardcoded.HardcodedWaypoints)
                });

                SaveChanges();

                static List<ItemDataCreateDto> ItemsToInsert(int i)
                {
                    var valueTuples = new List<(string, int)>
                    {
                        ("pietruszka", 1),
                        ("ziemniaki", 2),
                        ("orzeszki", 1),
                        ("jogurt naturalny", 1),
                        ("śmietana", 1),
                        ("woda", 2),
                        ("guma do żucia", 2)
                    };
                    return valueTuples.Map(tuple => new ItemDataCreateDto(i, tuple.Item1, tuple.Item2)).ToList();
                }

                base.CreateShoppingList(new ShoppingListCreateDto(userEntity.Id, "big-market"))
                    .Map(i => ItemsToInsert(i.Id))
                    .ForEach(i => i.ForEach(itemToAdd =>
                    {
                        base.AddItemToShoppingListDto(itemToAdd);
                        SaveChanges();
                    }));
            }
        }
    }
}