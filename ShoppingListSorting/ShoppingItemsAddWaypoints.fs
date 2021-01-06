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

    let pickSingleWaypoint (itemData: ItemDataWithPredictedType) (waypoints: ShopWaypointsReadDto) =
        (waypoints.Waypoints |> Seq.toList)
        |> List.tryFind (isWaypointMatched <| itemData)

    let pickWaypoints (shoppingList: ShoppingListWithDepartment) (waypoints: ShopWaypointsReadDto) =
        NotImplementedException
