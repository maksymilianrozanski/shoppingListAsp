namespace ShoppingListSorting

open System.Collections.Generic
open GroceryClassification
open Microsoft.Extensions.ML
open ShoppingData.ShoppingItemModule

module ShoppingItemsPredicting =

    type PredictedShopsDepartment = PredictedShopsDepartment of string

    type ItemDataWithPredictedType = ItemData * PredictedShopsDepartment

    type ShoppingListWithDepartment =
        { Id: int
          Name: string
          Password: string
          ShopsName: string
          Items: List<ItemDataWithPredictedType> }

    let predictShopDepartment (predictionEngine: PredictionEnginePool<GroceryData, GroceryItemPrediction>)
                              (itemData: ItemData)
                              =
        (itemData,
         predictionEngine
             .Predict(GroceryData.op_Implicit (GroceryToPredict(itemData.Name)))
             .FoodTypeLabel)
