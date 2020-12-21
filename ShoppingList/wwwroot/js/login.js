(function () {

    function saveCredentials(credentials) {
        sessionStorage.setItem("credentials", JSON.stringify(credentials));
    }

    const loginForm = document.forms[0];

    function readCredentials() {
        return {
            "name": document.getElementById("username").value,
            "password": document.getElementById("password").value,
            "id": document.getElementById("shoppingListId").value
        }
    }

    window.addEventListener("load", () => {
        forgetCredentials();

        loginForm.addEventListener("submit", event => {
            event.preventDefault();
            saveCredentials(readCredentials());
            window.location.replace("/addItems")
        });
    });
})
();