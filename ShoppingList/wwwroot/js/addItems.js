(function () {
    const formElement = document.forms[0];
    const addNewItem = async () => {
        // 1. read data from the form

        const credentials = credentialsFromStorage();

        const requestData =
            {
                "ShoppingListId": credentials.id,
                "Name": document.getElementById("name").value,
                "Quantity": document.getElementById("quantity").value,
                "ItemType": "ToBuy"
            };

        //todo: extract not successful response handling

        await fetch("/shoppingList/addItem", {
            method: 'POST', // *GET, POST, PUT, DELETE, etc.
            mode: 'cors', // no-cors, *cors, same-origin
            cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
            credentials: 'same-origin', // include, *same-origin, omit
            headers: {
                'Content-Type': 'application/json',
                "Authorization": authorizationHeaderValue()
            },
            redirect: 'follow',
            referrerPolicy: 'no-referrer',
            body: JSON.stringify(requestData)
        }).then(r => {
                if (r.status === 200) {
                    // 3. un-hide the alertElement when the request has been successful
                    console.log("success");
                    fetchUpdatedView();
                } else return handleFailure(r);
            }
        )
    };

    const displayListItems = async (responseJson) => {
        let response = await responseJson;

        let items = Array.from(response.items);

        let savedItems = document.getElementById("savedItemsList");
        savedItems.innerHTML = "";
        items.map(i => {
            let newListItem = document.createElement("li");
            newListItem.innerText = "id: " + i.id.toString() + ", " + i.name.toString() + " - " + i.quantity.toString();
            return newListItem;
        }).forEach(i => savedItems.appendChild(i));
    }

    function displayListItemsIfSuccessful(response) {
        if (response !== undefined && response.successful) {
            return displayListItems(response.content);
        }
    }

    const fetchUpdatedView = async () => {
        if (credentialsFromStorage() == null) {
            window.location.replace("/loginPage");
        } else {
            fetchShoppingList(credentialsFromStorage().id)
                .then(response => {
                    return displayListItemsIfSuccessful(response);
                });
        }
    }

    window.addEventListener("load", () => {
        fetchUpdatedView().then();

        formElement.addEventListener("submit", event => {
            event.preventDefault();
            addNewItem().then();
        });
    });
})
();