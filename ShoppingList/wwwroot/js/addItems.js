(function () {
    const alertElement = document.getElementById("success-alert");
    const formElement = document.forms[0];
    const addNewItem = async () => {
        // 1. read data from the form
        const requestData =
            {
                "ShoppingListId": formElement.elements[0].value,
                "Password": formElement.elements[1].value.toString(),
                "Name": formElement.elements[2].value.toString(),
                "Quantity": formElement.elements[3].value,
                "ItemType": "ToBuy"
            };
        // 2. call the application server using fetch method
        await fetch("/shoppingList/addItem", {
            method: 'POST', // *GET, POST, PUT, DELETE, etc.
            mode: 'cors', // no-cors, *cors, same-origin
            cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
            credentials: 'same-origin', // include, *same-origin, omit
            headers: {
                'Content-Type': 'application/json'
            },
            redirect: 'follow',
            referrerPolicy: 'no-referrer',
            body: JSON.stringify(requestData)
        }).then(r => {
                if (r.status === 200) {
                    // 3. un-hide the alertElement when the request has been successful
                    console.log("success");
                }
            }
        )
    };

    window.addEventListener("load", () => {
        formElement.addEventListener("submit", event => {
            event.preventDefault();
            addNewItem().then();
        });
    });
})
();