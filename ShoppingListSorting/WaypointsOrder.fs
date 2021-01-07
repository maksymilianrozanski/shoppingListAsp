namespace ShoppingListSorting

open ShoppingListSorting.ShoppingItemsAddWaypoints


module WaypointsOrder =

    let distinctAndDefinedWaypoints (shoppingList: ShoppingListWithWaypoints) =
        List.map (fun x ->
            (match x with
             | (_, _, w) -> w)) shoppingList.Items
        |> List.filter(fun x -> x.IsSome)
        |> List.distinct
