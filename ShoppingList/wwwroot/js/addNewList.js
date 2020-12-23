(function () {
    const formElement = document.forms[0];

    function createCredentials(name, password, id) {
        return {
            "name": name,
            "password": password,
            "id": id
        }
    }

    const addNewItem = async () => {
        const requestData =
            {
                "Name": document.getElementById("name").value,
                "Username": document.getElementById("username").value,
                "Password": document.getElementById("password").value,
            };
        await fetch("/create/createList", {
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
                    r.json().then(json => {
                        saveCredentials(createCredentials(requestData.Username, requestData.Password, json.id));
                        window.location.replace("/addItems");
                    })
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