namespace ShoppingData

open ShoppingData.ShoppingListModule
open ShoppingData.ShoppingItemModule

module ShoppingListModifyModule =


    let listItemToToBuy = modifyItem << (modifyStatus ToBuy)

    let listItemToLookingFor = modifyItem << (modifyStatus LookingFor)

    let listItemToBought = modifyItem << (modifyStatus Bought)

    let listItemToNotFound = modifyItem << (modifyStatus NotFound)

    let listItemToCancelled = modifyItem << (modifyStatus Cancelled)
