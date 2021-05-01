function collectItemsPage(currentShoppingListId, currentUsername) {
    const itemActionsIds = {
        ItemLookingFor: 0,
        ItemToNotFound: 1,
        ItemToBought: 2,
        ItemToCancelled: 3
    }

    const toBuyColor = "#f5f5f5";
    const lookingForColor = "#c8fff5";
    const boughtColor = "#edfced";
    const notFoundColor = "#faeed5";
    const cancelledColor = "#c8c8c8";

    const actionButtons = [
        itemActionsIds.ItemLookingFor,
        itemActionsIds.ItemToBought,
        itemActionsIds.ItemToNotFound,
        itemActionsIds.ItemToCancelled];

    const actionButtonColor = (actionId) => {
        switch (actionId) {
            case itemActionsIds.ItemLookingFor:
                return lookingForColor
            case itemActionsIds.ItemToNotFound:
                return notFoundColor
            case itemActionsIds.ItemToBought:
                return boughtColor
            case itemActionsIds.ItemToCancelled:
                return cancelledColor
        }
    }

    const itemActionText = (actionId) => {
        switch (actionId) {
            case itemActionsIds.ItemLookingFor:
                return "Search for";
            case itemActionsIds.ItemToNotFound:
                return "Not found";
            case itemActionsIds.ItemToBought:
                return "Collected";
            case itemActionsIds.ItemToCancelled:
                return "Cancel item";
        }
    }

    const itemTypeBackgroundColor = (itemDataReadDto) => {
        switch (itemDataReadDto.itemType) {
            case "ToBuy" :
                return toBuyColor;
            case "LookingFor":
                return lookingForColor;
            case "Bought":
                return boughtColor;
            case "NotFound":
                return notFoundColor;
            case "Cancelled":
                return cancelledColor;
        }
    }

    async function postItemAction(itemDataActionDto) {
        return Promise.resolve(
            await fetch("/protected/modifyItem", {
                method: 'POST',
                mode: "cors",
                cache: "no-cache",
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/json',
                },
                redirect: 'follow',
                referrerPolicy: 'no-referrer',
                body: JSON.stringify(itemDataActionDto())
            }).then(r => {
                if (r.status === 200) {
                    return {
                        successful: true,
                        content: r.json()
                    }
                } else {
                    return handleFailure(r);
                }
            })
        );
    }

    function shouldBeDisabled(itemType, actionNumber) {
        return (actionNumber === itemActionsIds.ItemLookingFor && itemType.startsWith("LookingFor") ||
            actionNumber === itemActionsIds.ItemToNotFound && itemType.startsWith("NotFound") ||
            actionNumber === itemActionsIds.ItemToBought && itemType.startsWith("Bought") ||
            actionNumber === itemActionsIds.ItemToCancelled && itemType.startsWith("Cancelled"));
    }

    const createItemActionButton = (itemDataReadDto) => (actionNumber) => {
        const actionButton = document.createElement("button");
        actionButton.innerText = itemActionText(actionNumber);

        if (shouldBeDisabled(itemDataReadDto.itemType, actionNumber)) {
            actionButton.disabled = true;
        }

        const actionDto = () =>
            ({
                user: currentUsername,
                itemId: itemDataReadDto.id,
                shoppingListId: currentShoppingListId,
                actionNumber: actionNumber
            });

        actionButton.addEventListener("click", async () => postItemAction(actionDto)
            .then(r => {
                return displayItemsIfSuccessful(r);
            })
        );
        return actionButton;
    }

    const itemButtons = (itemDataReadDto) => {
        const actions = Array.from(actionButtons);
        return actions.map(i => createItemActionButton(itemDataReadDto)(i));
    }

    const itemWithActions = (itemDataReadDto) => {
        const liElement = document.createElement("li");

        const displayedText = document.createElement("div")
        displayedText.innerText = itemDataReadDto.name.toString() + " - " + itemDataReadDto.quantity.toString();
        liElement.appendChild(displayedText);

        const buttonsContainer = document.createElement("div");
        liElement.appendChild(buttonsContainer);

        itemButtons(itemDataReadDto).forEach(i => buttonsContainer.appendChild(i));
        return liElement;
    }

    async function displayItems(responseContent) {
        let itemsAwaited = await responseContent;

        const olRoot = document.getElementById("shoppingListItems");
        olRoot.innerHTML = "";

        Array.from(itemsAwaited.items)
            .map(i => itemWithActions(i))
            .forEach(i => olRoot.appendChild(i));
    }

    function displayItemsIfSuccessful(response) {
        if (response !== undefined && response.successful) {
            return displayItems(response.content);
        }
    }

    const fetchShoppingList = async () => {
        return Promise.resolve(
            await fetch("/protected/" + currentShoppingListId, {
                method: 'GET',
                mode: "cors",
                cache: "no-cache",
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/json',
                },
                redirect: 'follow',
                referrerPolicy: 'no-referrer'
            }).then(r => {
                if (r.status === 200) {
                    console.log("posting item action successful");
                    return {
                        successful: true,
                        content: r.json()
                    }
                } else {
                    return handleFailure(r);
                }
            }))
    }

    const fetchUpdatedView = async () => {
        fetchShoppingList()
            .then(response => {
                return displayItemsIfSuccessful(response);
            });
    }

    window.addEventListener("load", () => {
            fetchUpdatedView().then();
        }
    );
}
