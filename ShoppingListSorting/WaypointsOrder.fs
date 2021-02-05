namespace ShoppingListSorting

open System
open FSharpPlus
open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListModule
open ShoppingListSorting.ShoppingItemsAddWaypoints

open Waypoints
open WaypointsModule
open ShoppingItemsPredicting

module WaypointsOrder =

    let distinctAndDefinedWaypoints (shoppingList: ShoppingListWithWaypoints) =
        List.map (fun x ->
            (match x with
             | (_, _, w) -> w)) shoppingList.Items
        |> List.filter (fun x -> x.IsSome)
        |> List.distinct
        |> List.choose id

    let private indexDictionary (items: list<'a>) =
        List.zip items ({ 1 .. items.Length } |> Seq.toList)
        |> dict

    let sortedWaypointNames (shoppingList: ShoppingListWithWaypoints) =
        match shoppingList.StartAndCheckout with
        | (start, checkout) ->
            [ start ]
            @ (distinctAndDefinedWaypoints shoppingList)
              @ [ checkout ]
        |> Array.ofList
        |> waypointNamesSorted
        // drops first item which is starting point
        |> fun x -> x.Tail

    let sortShoppingListItems (shoppingList: ShoppingListWithWaypoints) =
        let dictionary =
            sortedWaypointNames shoppingList
            |> indexDictionary

        let sortedItems =
            List.sortBy (fun x ->
                match x with
                | (_, _, Some (y)) ->
                    match dictionary.TryGetValue y.name with
                    | true, value -> value
                    | _ -> Int32.MaxValue
                | (_, _, None) -> Int32.MaxValue) shoppingList.Items
            |> List.map (fun i ->
                match i with
                | (itemData, _, _) -> itemData)


        { Password = shoppingList.Password
          Items = sortedItems
          Id = shoppingList.Id }: ShoppingList

    /// puts items which ItemType which should not be included in sorting is 2nd tuple value
    let private partitionList (shoppingList: ShoppingList) =
        let allItems = shoppingList.Items

        let (toSort, toSkip) =
            List.partition (fun i ->
                (i.ItemType = ItemType.ToBuy
                 || i.ItemType = ItemType.LookingFor)) allItems

        let result = { shoppingList with Items = toSort }
        (result, toSkip)

    let sortShoppingList predictingFun waypoints shoppingList =
        let (list, toSkip) = partitionList shoppingList

        predictAllItems predictingFun list
        |> addWaypointsToShoppingList waypoints
        |> sortShoppingListItems
        |> fun x -> { x with Items = x.Items @ toSkip }
