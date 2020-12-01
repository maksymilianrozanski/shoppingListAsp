namespace ShoppingData

open FSharpPlus
open ShoppingData.ShoppingItemModule
open ShoppingData

module ShoppingListModule =

    type ShoppingList =
        { Id: int
          Name: string
          Password: string
          Items: List<ItemData> }

    let emptyShoppingList name id password =
        { Name = name
          Password = password
          Items = List.empty
          Id = id }

    let executeIfPassword f (list: ShoppingList) password =
        if (list.Password = password) then Choice.flatten (Choice1Of2(f (list))) else Choice2Of2(IncorrectPassword)

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
        | None -> Choice2Of2(ListItemNotFound)

    let modifyItemIfPassword f id = executeIfPassword (modifyItem f id)

    type ItemModifiedByUser = string -> ItemData -> Choice<ItemData, ShoppingListErrors>

    let listItemToAssigned = assignItem >> modifyItemIfPassword

    let listItemToNotFound = notFoundItem >> modifyItemIfPassword

    let listItemToBought = toBought >> modifyItemIfPassword

    let listItemToCancelled = toCancelled >> modifyItemIfPassword
