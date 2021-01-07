namespace ShoppingListSorting

open System
open FSharpPlus
open ShoppingData.ShoppingItemModule
open ShoppingList.Dtos
open ShoppingListSorting.ShoppingItemsPredicting
open Waypoints.WaypointsModule

module ShoppingItemsAddWaypoints =

    let private isWaypointMatched (itemData: ItemDataWithPredictedType) (waypoint: Waypoint) =
        match itemData with
        | (_, PredictedShopsDepartment (b)) -> b = waypoint.name

    let pickSingleWaypoint (waypoints: ShopWaypointsReadDto) (itemData: ItemDataWithPredictedType) =
        (waypoints.Waypoints |> Seq.toList)
        |> List.tryFind (isWaypointMatched <| itemData)

    type ItemDataWithPredictedTypeWithWaypoint = ItemData * PredictedShopsDepartment * Waypoint option

    let private addWaypointToItemData (waypoints: ShopWaypointsReadDto) (itemData: ItemDataWithPredictedType) =
        match itemData with
        | (data, predicted) -> (data, predicted, pickSingleWaypoint waypoints itemData)

    type ShoppingListWithWaypoints =
        { Id: int
          Name: string
          Password: string
          ShopName: string
          Items: ItemDataWithPredictedTypeWithWaypoint list }

    let private pickWaypoints (shoppingList: ShoppingListWithDepartment) (waypoints: ShopWaypointsReadDto) =
        List.map (addWaypointToItemData waypoints) (shoppingList.Items |> Seq.toList)

    let addWaypointsToShoppingList (shoppingList: ShoppingListWithDepartment) (waypoints: ShopWaypointsReadDto) =
        { Id = shoppingList.Id
          Name = shoppingList.Name
          Password = shoppingList.Password
          ShopName = shoppingList.ShopName
          Items = pickWaypoints shoppingList waypoints }
