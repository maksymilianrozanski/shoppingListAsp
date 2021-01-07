namespace ShoppingListSorting

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

    let private indexDictionary (items: list<'a>) =
        List.zip items ({ 0 .. items.Length } |> Seq.toList)
        |> dict

//    let sortWaypoints (shoppingList: ShoppingListWithWaypoints) =
//        let waypointsWithStartAndEnd =
//            shoppingList.