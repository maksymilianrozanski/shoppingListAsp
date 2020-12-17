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
                    body: JSON.stringify(itemDataActionDto)
                }).then(r => {
                    if (r.status === 200) {
                        console.log("posting item action successful");
                        return r.json();
                    }
                })
            );
        }

        const createItemActionButton = (userNameAndPassword) => (itemDataReadDto) => (actionId) => {
            const actionButton = document.createElement("button")
            actionButton.innerText = itemActionText(actionId);
            const actionDto = {
                user: userNameAndPassword.name,
                itemId: itemDataReadDto.id,
                //todo: pass real shoppingListId
                shoppingListId: 1,
                password: userNameAndPassword.password,
                actionNumber: actionId
            }
            actionButton.addEventListener("click", async () => postItemAction(actionDto)
                .then(r => displayItems(r.items))
            );
            return actionButton;
        }

        const itemButtons = (userNameAndPassword) => (itemDataReadDto) => {
            const itemType = itemDataReadDto.itemType;
            const actions = Array.from(getAllowedActions(itemType));
            return actions.map(i => createItemActionButton(userNameAndPassword)(itemDataReadDto)(i));
        }

        const itemWithActions = (userNameAndPassword) => (itemDataReadDto) => {
            const liElement = document.createElement("li");

            const displayedText = document.createElement("a")
            displayedText.innerText = "id: " + itemDataReadDto.id.toString() + ", " + itemDataReadDto.name.toString() + " - " + itemDataReadDto.quantity.toString();
            liElement.appendChild(displayedText);

            itemButtons(userNameAndPassword)(itemDataReadDto).forEach(i => liElement.appendChild(i));
            return liElement;
        }

        async function displayItems(items) {
            const olRoot = document.getElementById("shoppingListItems");
            olRoot.innerHTML = "";

            const password = document.getElementById("password").value;
            const username = document.getElementById("username").value;
            const userNameAndPassword = {name: username, password: password};

            Array.from(items)
                .map(i => itemWithActions(userNameAndPassword)(i))
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