namespace GroceryClassification
{
    using Microsoft.ML.Data;

    public class GroceryData
    {
        [LoadColumn(0)] public string FoodTypeLabel { get; set; }

        [LoadColumn(1)] public string FoodNameFeature { get; set; }

        public static implicit operator GroceryData(GroceryToPredict groceryToPredict) =>
            new()
            {
                FoodNameFeature = groceryToPredict.FoodNameFeature,
                FoodTypeLabel = "unknown"
            };
    }

    public class GroceryToPredict
    {
        public string FoodNameFeature { get; set; }

        public GroceryToPredict(string foodNameFeature)
        {
            FoodNameFeature = foodNameFeature;
        }
    }

    public class GroceryItemPrediction
    {
        [ColumnName("PredictedLabel")] public string FoodTypeLabel { get; set; }
    }
}