namespace ShoppingListSorting

open FSharpPlus
open SharedTypes.Dtos
open ShoppingData.ShoppingItemModule
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
          Password: string
          StartAndCheckout: (Waypoint * Waypoint)
          Items: ItemDataWithPredictedTypeWithWaypoint list }

    let private pickWaypoints (shoppingList: ShoppingListWithDepartment) (waypoints: ShopWaypointsReadDto) =
        List.map (addWaypointToItemData waypoints) (shoppingList.Items |> Seq.toList)

    let addWaypointsToShoppingList (waypoints: ShopWaypointsReadDto) (shoppingList: ShoppingListWithDepartment) =
        { Id = shoppingList.Id
          Password = shoppingList.Password
          StartAndCheckout = (waypoints.Start, waypoints.Checkout)
          Items = pickWaypoints shoppingList waypoints }
