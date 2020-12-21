(function () {
        const itemActionsIds = {
            AssignItem: 0,
            ItemToNotFound: 1,
            ItemToBought: 2,
            ItemToCancelled: 3
        }

        const getAllowedActions = (itemType) => {
            if (itemType.startsWith("ToBuy"))
                return [itemActionsIds.AssignItem, itemActionsIds.ItemToCancelled];
            else if (itemType.startsWith("Assigned"))
                return [itemActionsIds.ItemToBought, itemActionsIds.ItemToNotFound, itemActionsIds.ItemToCancelled];
            else if (itemType.startsWith("Bought"))
                return [];
            else if (itemType.startsWith("NotFound"))
                return [itemActionsIds.AssignItem, itemActionsIds.ItemToCancelled];
            else if (itemType.startsWith("Cancelled"))
                return [];
        }

        const itemActionText = (actionId) => {
            switch (actionId) {
                case itemActionsIds.AssignItem:
                    return "Assign item";
                case itemActionsIds.ItemToNotFound:
                    return "Not found";
                case itemActionsIds.ItemToBought:
                    return "Item found";
                case itemActionsIds.ItemToCancelled:
                    return "Cancel item";
            }
        }

        const itemTypeDisplayedText = (itemDataActionDto) => {
            const itemType = itemDataActionDto.itemType;

            if (itemType === "ToBuy")
                return "To buy";
            else if (itemType.startsWith("Assigned"))
                return "Assigned to " + itemType.substring(9);
            else if (itemType === "Bought")
                return "Bought";
            else if (itemType === "NotFound")
                return "Not found";
            else if (itemType === "Cancelled")
                return "Cancelled";
            else return "unknown";
        }

        async function postItemAction(itemDataActionDto) {
            return Promise.resolve(
                await fetch("/shoppingList/modifyItem", {
                    method: 'POST',
                    mode: "cors",
                    cache: "no-cache",
                    credentials: 'same-origin',
                    headers: {
                        'Content-Type': 'application/json',
                        "Authorization": authorizationHeaderValue()
                    },
                    redirect: 'follow',
                    referrerPolicy: 'no-referrer',
                    body: JSON.stringify(itemDataActionDto())
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
                })
            );
        }

        const getShoppingListId = () => credentialsFromStorage().id;

        const getUsername = () => credentialsFromStorage().name;

        const getPassword = () => credentialsFromStorage().password;

        const createItemActionButton = (itemDataReadDto) => (actionId) => {
            const actionButton = document.createElement("button")
            actionButton.innerText = itemActionText(actionId);

            const actionDto = () =>
                ({
                    user: getUsername(),
                    itemId: itemDataReadDto.id,
                    shoppingListId: getShoppingListId(),
                    password: getPassword(),
                    actionNumber: actionId
                });

            actionButton.addEventListener("click", async () => postItemAction(actionDto)
                .then(r => {
                    return displayItemsIfSuccessful(r);
                })
            );
            return actionButton;
        }

        const itemButtons = (itemDataReadDto) => {
            const itemType = itemDataReadDto.itemType;
            const actions = Array.from(getAllowedActions(itemType));
            return actions.map(i => createItemActionButton(itemDataReadDto)(i));
        }

        const itemWithActions = (itemDataReadDto) => {
            const liElement = document.createElement("li");

            const displayedText = document.createElement("a")
            displayedText.innerText = "id: " + itemDataReadDto.id.toString() + ", " + itemDataReadDto.name.toString() +
                " - " + itemDataReadDto.quantity.toString() + ", " + itemTypeDisplayedText(itemDataReadDto);
            liElement.appendChild(displayedText);

            itemButtons(itemDataReadDto).forEach(i => liElement.appendChild(i));
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

        const fetchUpdatedView = async () => {
            if (credentialsFromStorage() == null) {
                window.location.replace("/loginPage");
            } else {
                fetchShoppingList(credentialsFromStorage().id)
                    .then(response => {
                        return displayItemsIfSuccessful(response);
                    });
            }
        }

        window.addEventListener("load", () => {
                fetchUpdatedView().then();

                document.getElementById("fetchListButton").addEventListener("click", event => {
                    event.preventDefault();
                    fetchUpdatedView().then();
                });
            }
        );
    }
)
();