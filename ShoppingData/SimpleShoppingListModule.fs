namespace ShoppingData

open ShoppingData
open SimpleShoppingItemModule
open ShoppingListErrors

module SimpleShoppingListModule =

    type ShoppingList =
        { Id: int
          Password: string
          ShopName: string
          Items: List<ItemData> }

    let emptyShoppingList id password =
        { Password = password
          Items = List.Empty
          Id = id
          ShopName = "" }
        
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
        