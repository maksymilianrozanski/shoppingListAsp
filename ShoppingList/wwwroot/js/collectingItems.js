(function () {
    const alertElement = document.getElementById("success-alert");
    const shoppingListNameAndPassword = document.forms[0];

    const itemActionsIds = {
        AssignItem: 0,
        ItemToNotFound: 1,
        ItemToBought: 2,
        ItemToCancelled: 3
    }

    const getAllowedActions2 = (actionText) => {
        if (actionText.startsWith("ToBuy"))
            return [itemActionsIds.AssignItem, itemActionsIds.ItemToCancelled];
        else if (actionText.startsWith("Assigned"))
            return [itemActionsIds.ItemToBought, itemActionsIds.ItemToNotFound, itemActionsIds.ItemToCancelled];
        else if (actionText.startsWith("Bought"))
            return [];
        else if (actionText.startsWith("NotFound"))
                return [itemActionsIds.AssignItem, itemActionsIds.ItemToCancelled];
        else if (actionText.startsWith("Cancelled"))
            return [];
    }

    const itemWithActions = (itemDataReadDto) => {
        const liElement = document.createElement("li");
        const displayedText = document.createElement("a")
        displayedText.innerText = "id: " + itemDataReadDto.id.toString() + ", " + itemDataReadDto.name.toString() + " - " + itemDataReadDto.quantity.toString();
        liElement.appendChild(displayedText);
        const allowedActionsTextForDto = getAllowedActions2(itemDataReadDto.itemType.toString())
        const allowedActionsElement = document.createElement("div")
        allowedActionsElement.innerText = allowedActionsTextForDto.toString();
        liElement.appendChild(allowedActionsElement)
        return liElement;
    }

    const displayItems = async (items) => {
        const olRoot = document.getElementById("shoppingListItems");
        olRoot.innerHTML = "";

        Array.from(items)
            .map(i => itemWithActions(i))
            .forEach(i => olRoot.appendChild(i));
    }

    window.addEventListener("load", () => {
        shoppingListNameAndPassword.addEventListener("submit", event => {
            event.preventDefault();
            fetchShoppingList(shoppingListNameAndPassword.elements[0].value)
                .then(r => displayItems(r.items));
        });
    });
})
();