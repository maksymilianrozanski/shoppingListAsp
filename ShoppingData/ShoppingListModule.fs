namespace ShoppingData

open FSharpPlus
open ShoppingData.ShoppingItemModule
open ShoppingData.ShoppingListErrors

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

    let private containsItemWithId (list: ShoppingList) (item: ItemData) =
        list.Items
        |> List.exists (fun i -> i.Id = item.Id)

    let addItemIfNotExist item (list: ShoppingList) =
        match (containsItemWithId list item) with
        | false -> Choice1Of2(addItem list item)
        | true -> Choice2Of2(ItemWithIdAlreadyExists)

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

    let listItemToAssigned = assignItem >> modifyItemIfPassword

    let listItemToNotFound = notFoundItem >> modifyItemIfPassword

    let listItemToBought = toBought >> modifyItemIfPassword

    let listItemToCancelled = toCancelled >> modifyItemIfPassword

    let addItemIfPassword = executeIfPassword << addItemIfNotExist
