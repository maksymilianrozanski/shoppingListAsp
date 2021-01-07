namespace ShoppingListSorting

open FSharpPlus.Control
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
        List.zip items ({ 0 .. items.Length } |> Seq.toList)
        |> dict

    let sortWaypoints (shoppingList: ShoppingListWithWaypoints) =
        match shoppingList.StartAndCheckout with
        | (start, checkout) ->
            [ start ]
            @ (distinctAndDefinedWaypoints shoppingList)
              @ [ checkout ]
        |> Array.ofList
        |> waypointNamesSorted
        |> fun x -> x.Tail
