using System;
using GroceryClassification;
using Microsoft.Extensions.ML;
using ShoppingData;

namespace ShoppingList.Utils
{
    public static class PredictionAdapter
    {
        public static Func<PredictionEnginePool<GroceryData, GroceryItemPrediction>, ShoppingItemModule.ItemData, string> PredictionFunc =>
            (pool, itemData) => pool.Predict(modelName: "GroceryModel", example: new GroceryToPredict(itemData.Name))
                .FoodTypeLabel;
    }
}