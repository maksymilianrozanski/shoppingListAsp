namespace GroceryClassification
{
    using Microsoft.ML.Data;

    public class GroceryData
    {
        [LoadColumn(0)] public string FoodTypeLabel { get; set; }

        [LoadColumn(1)] public string FoodNameFeature { get; set; }
    }

    public class GroceryItemPrediction
    {
        [ColumnName("PredictedLabel")] public string FoodTypeLabel { get; set; }
    }
}