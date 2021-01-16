namespace ShoppingData

open ShoppingData.ShoppingListModule
open ShoppingData.ShoppingItemModule

module ShoppingListModifyModule =

    let listItemToAssigned = modifyItem << assignItem

    let listItemToNotFound = modifyItem << notFoundItem

    let listItemToBought = modifyItem << toBought

    let listItemToCancelled = modifyItem << toCancelled

module SimpleShoppingListModifyModule =

    let listItemToToBuy =
        SimpleShoppingListModule.modifyItem
        << (SimpleShoppingItemModule.modifyStatus SimpleShoppingItemModule.ToBuy)

    let listItemToLookingFor =
        SimpleShoppingListModule.modifyItem
        << (SimpleShoppingItemModule.modifyStatus SimpleShoppingItemModule.LookingFor)

    let listItemToBought =
        SimpleShoppingListModule.modifyItem
        << (SimpleShoppingItemModule.modifyStatus SimpleShoppingItemModule.Bought)

    let listItemToNotFound =
        SimpleShoppingListModule.modifyItem
        << (SimpleShoppingItemModule.modifyStatus SimpleShoppingItemModule.NotFound)

    let listItemToCancelled =
        SimpleShoppingListModule.modifyItem
        << (SimpleShoppingItemModule.modifyStatus SimpleShoppingItemModule.Cancelled)
