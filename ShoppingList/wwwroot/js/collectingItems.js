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

        const createItemActionButton = (itemDataReadDto) => (actionId) => {
            const actionButton = document.createElement("button")
            actionButton.innerText = itemActionText(actionId);
            //todo: add sending data on button click
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
            displayedText.innerText = "id: " + itemDataReadDto.id.toString() + ", " + itemDataReadDto.name.toString() + " - " + itemDataReadDto.quantity.toString();
            liElement.appendChild(displayedText);

            itemButtons(itemDataReadDto).forEach(i => liElement.appendChild(i));
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
    }
)
();