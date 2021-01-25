namespace ShoppingData

open FSharpPlus
open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListErrors

module ShoppingListModule =

    type ShoppingList =
        { Id: int
          Password: string
          ShopName: string
          Items: List<ItemData> }

    let emptyShoppingList id password =
        { Password = password
          Items = List.empty
          Id = id
          ShopName = "" }

    let addItem (list: ShoppingList) item =
        { list with
              Items = list.Items @ [ item ] }

    let modifyItem f itemId (list: ShoppingList) =
        list.Items
        |> List.tryFind (fun i -> i.Id = itemId)
        |> function
        | Some (itemFound) ->
            match f (itemFound) with
            | Choice1Of2 (itemModified) ->
                { list with
                      Items =
                          list.Items
                          |> List.map (fun i -> if (i.Id = itemId) then itemModified else i) }
                |> Choice1Of2
            | Choice2Of2 (failure) -> Choice2Of2(failure)
        | None -> Choice2Of2(NotFound)
