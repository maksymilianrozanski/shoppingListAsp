using System;
using System.IO;
using Microsoft.ML;

namespace GroceryClassification
{
    class Program
    {
        static void Main(string[] args)
        {
            var mlContext = new MLContext(seed: 0);

            var appPath = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            var trainDataPath = Path.Combine(appPath, "..", "..", "..", "Data", "SELL_4_training.tsv");
            var testDataPath = Path.Combine(appPath, "..", "..", "..", "Data", "SELL_4_test.tsv");
            var modelPath = Path.Combine(appPath, "..", "..", "..", "Models", "model.zip");

            var predictor =
                new GenericPredictor<GroceryData, GroceryItemPrediction>(mlContext, trainDataPath, testDataPath,
                    modelPath);

            predictor.CreateAndSaveModel();

            Console.WriteLine(predictor.PredictLabel(new GroceryData() {FoodNameFeature = "KAWA"}).FoodTypeLabel);
            Console.WriteLine(predictor.PredictLabel(new GroceryData() {FoodNameFeature = "JOGURT"}).FoodTypeLabel);
            Console.WriteLine(predictor.PredictLabel(new GroceryData() {FoodNameFeature = "CYTRYNA"}).FoodTypeLabel);
            Console.WriteLine(predictor.PredictLabel(new GroceryData() {FoodNameFeature = "ORZESZKI"}).FoodTypeLabel);
        }
    }
}