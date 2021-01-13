namespace ShoppingData

open ShoppingData.ShoppingListModule
open ShoppingData.ShoppingItemModule

module ShoppingListModifyModule =

    let listItemToAssigned = modifyItem << assignItem

    let listItemToNotFound = modifyItem << notFoundItem

    let listItemToBought = modifyItem << toBought

    let listItemToCancelled = modifyItem << toCancelled
