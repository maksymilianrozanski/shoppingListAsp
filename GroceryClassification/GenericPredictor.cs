using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace GroceryClassification
{
    internal class GenericPredictor<T1, T2> where T1 : class where T2 : class, new()
    {
        private readonly MLContext _mlContext;
        private readonly string _testDataPath;
        private readonly IDataView _trainingDataView;
        private readonly string _modelPath;

        public GenericPredictor(MLContext mlContext, string trainDataPath, string testDataPath, string modelPath)
        {
            _mlContext = mlContext;
            _testDataPath = testDataPath;
            _modelPath = modelPath;
            _trainingDataView = _mlContext.Data.LoadFromTextFile<T1>(trainDataPath, hasHeader: true);
        }

        public void CreateAndSaveModel()
        {
            var pipeline = ProcessData(_mlContext);
            var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline, _mlContext);
            Evaluate(_trainingDataView.Schema, _mlContext, _testDataPath, trainingPipeline, _modelPath);
        }

        public T2 PredictLabel(T1 data)
        {
            ITransformer loadedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);
            return _mlContext.Model.CreatePredictionEngine<T1, T2>(loadedModel)
                .Predict(data);
        }

        private static IEstimator<ITransformer> ProcessData(MLContext mlContext) =>
            mlContext.Transforms.Conversion
                .MapValueToKey(inputColumnName: "FoodTypeLabel", outputColumnName: "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "FoodNameFeature",
                    outputColumnName: "FoodNameFeaturized"))
                .Append(mlContext.Transforms.Concatenate("Features",
                    "FoodNameFeaturized"))
                .AppendCacheCheckpoint(mlContext);

        private static void Evaluate(DataViewSchema trainingDataViewSchema, MLContext mlContext,
            string testDataPath,
            ITransformer trainedModel, string modelPath)
        {
            var testDataView = mlContext.Data.LoadFromTextFile<T1>(testDataPath, hasHeader: true);

            var testMetrics = mlContext.MulticlassClassification.Evaluate(trainedModel.Transform(testDataView));

            Console.WriteLine(
                $"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Multi-class Classification model - Test Data     ");
            Console.WriteLine(
                $"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       MicroAccuracy:    {testMetrics.MicroAccuracy:0.###}");
            Console.WriteLine($"*       MacroAccuracy:    {testMetrics.MacroAccuracy:0.###}");
            Console.WriteLine($"*       LogLoss:          {testMetrics.LogLoss:#.###}");
            Console.WriteLine($"*       LogLossReduction: {testMetrics.LogLossReduction:#.###}");
            Console.WriteLine(
                $"*************************************************************************************************************");

            SaveModelAsFile(mlContext, trainingDataViewSchema, trainedModel, modelPath);
        }

        private static void SaveModelAsFile(MLContext mlContext, DataViewSchema trainingDataViewSchema,
            ITransformer trainedModel, string modelPath) =>
            mlContext.Model.Save(trainedModel, trainingDataViewSchema, modelPath);

        private static TransformerChain<KeyToValueMappingTransformer> BuildAndTrainModel(IDataView trainingDataView,
            IEstimator<ITransformer> pipeline, MLContext mlContext) =>
            pipeline
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel")).Fit(trainingDataView);
    }
}