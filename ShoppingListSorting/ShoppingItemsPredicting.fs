namespace ShoppingListSorting

open System.Collections.Generic
open FSharpPlus
open GroceryClassification
open Microsoft.Extensions.ML
open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListModule

module ShoppingItemsPredicting =

    type PredictedShopsDepartment = PredictedShopsDepartment of string

    type ItemDataWithPredictedType = ItemData * PredictedShopsDepartment

    type ShoppingListWithDepartment =
        { Id: int
          Name: string
          Password: string
          ShopName: string
          Items: List<ItemDataWithPredictedType> }

    let predictUsingEngine (predictionEngine: PredictionEnginePool<GroceryData, GroceryItemPrediction>) itemName =
        predictionEngine.Predict(GroceryData.op_Implicit (GroceryToPredict(itemName)))

    let addPredictionToItemData (predictingFun: ItemData -> string) (itemData: ItemData) =
        (itemData, PredictedShopsDepartment(predictingFun (itemData)))

    let predictShopDepartments (predictingFun: ItemData -> string) items =
        List.map (addPredictionToItemData predictingFun) items

    let predictAllItems (predictingFun: ItemData -> string) (shoppingList: ShoppingList) =
        { Id = shoppingList.Id
          Name = shoppingList.Name
          Password = shoppingList.Password
          //todo: add ShopName field to ShoppingList
          ShopName = "todo"
          Items =
              predictShopDepartments predictingFun shoppingList.Items
              |> ResizeArray }
