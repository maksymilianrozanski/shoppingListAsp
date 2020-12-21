(function () {
    const formElement = document.forms[0];
    const addNewItem = async () => {
        // 1. read data from the form
        const requestData =
            {
                "Name": formElement.elements[0].value.toString(),
                "Password": formElement.elements[1].value.toString(),
            };
        // 2. call the application server using fetch method
        await fetch("/shoppingList/createList", {
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
                if (r.status === 201 || r.status === 200) {
                    // 3. un-hide the alertElement when the request has been successful
                    // const jsonResponse = r.json();
                    // sessionStorage.setItem(jsonResponse.id, jsonResponse.password)
                    window.location.replace("/shoppingList/addItems")
                    console.log("success");
                } else return handleFailure(r);
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