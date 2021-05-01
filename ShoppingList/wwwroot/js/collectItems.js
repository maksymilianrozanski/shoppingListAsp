function collectItemsPage(currentShoppingListId, currentUsername) {
    const itemActionsIds = {
        ItemLookingFor: 0,
        ItemToNotFound: 1,
        ItemToBought: 2,
        ItemToCancelled: 3
    }

    const actionButtons = [
        itemActionsIds.ItemLookingFor,
        itemActionsIds.ItemToBought,
        itemActionsIds.ItemToNotFound,
        itemActionsIds.ItemToCancelled];

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

    const addLineThrough = (color) => (element) => {
        element.style.textDecoration = 'line-through';
        element.style.textDecorationColor = color;
        element.style.textDecorationThickness = '6px'
        return element;
    }

    const boughtStyle = addLineThrough("rgba(156,255,156,0.75)")
    const cancelledStyle = addLineThrough("rgba(255,255,255,0.8)")
    const notFoundStyle = (element) => {
        element.style.textDecoration = 'underline dotted';
        element.style.textDecorationColor = "rgba(231,18,18,0.7)";
        element.style.textDecorationThickness = '4px';
        return element;
    }
    const lookingForStyle = (element) => {
        element.innerHTML = "<mark style='background-color: rgba(255,255,0,0.7); border-radius: 20%; padding: 4px'>" + element.innerText + "</mark>";
        return element;
    }

    const displayedTextItem = (itemDataReadDto) => {

        const displayedTextElement = document.createElement("div")
        displayedTextElement.innerText = itemDataReadDto.name.toString() + " - " + itemDataReadDto.quantity.toString();

        switch (itemDataReadDto.itemType) {
            case "LookingFor":
                return lookingForStyle(displayedTextElement);
            case "Bought":
                return boughtStyle(displayedTextElement)
            case "NotFound":
                return notFoundStyle(displayedTextElement);
            case "Cancelled":
                return cancelledStyle(displayedTextElement);
            default :
                return displayedTextElement;
        }
    }

    const itemWithActions = (itemDataReadDto) => {
        const liElement = document.createElement("li");

        const displayedText = displayedTextItem(itemDataReadDto);
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
