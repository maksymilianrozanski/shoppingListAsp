(function () {
        const alertElement = document.getElementById("success-alert");
        const shoppingListNameAndPassword = document.forms[0];

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
                        'Content-Type': 'application/json'
                    },
                    redirect: 'follow',
                    referrerPolicy: 'no-referrer',
                    body: JSON.stringify(itemDataActionDto())
                }).then(r => {
                    if (r.status === 200) {
                        console.log("posting item action successful");
                        return r.json();
                    }
                })
            );
        }

        const getShoppingListId = () => document.getElementById("shoppingListId").value;

        const getUsername = () => document.getElementById("username").value;

        const getPassword = () => document.getElementById("password").value;

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
                .then(r => displayItems(r.items))
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

        async function displayItems(items) {
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
    }
)
();