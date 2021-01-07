namespace ShoppingListSorting

open System
open FSharpPlus.Control
open ShoppingData.ShoppingListModule
open ShoppingListSorting.ShoppingItemsAddWaypoints

open Waypoints
open WaypointsModule

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

    //todo: add test case
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
     
        { Name = shoppingList.Name
          Password = shoppingList.Password
          Items = sortedItems
          Id = shoppingList.Id }
