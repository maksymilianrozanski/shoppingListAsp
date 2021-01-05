using GroceryClassification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;

namespace ShoppingList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PredictController : ControllerBase
    {
        private readonly PredictionEnginePool<GroceryData, GroceryItemPrediction> _predictionEnginePool;

        public PredictController(PredictionEnginePool<GroceryData, GroceryItemPrediction> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        [HttpPost]
        [Route("predictItem")]
        public ActionResult<string> Post(GroceryData input)
        {
            if (!ModelState.IsValid) return BadRequest();
            else
            {
                var prediction = _predictionEnginePool.Predict(modelName: "GroceryModel", example: input);
                return Ok(prediction.FoodTypeLabel);
            }
        }
    }
}