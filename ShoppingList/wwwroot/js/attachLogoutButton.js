window.addEventListener("load", () => {

    const aElement = document.createElement("a");
    aElement.className = "nav-link text-dark";
    aElement.setAttribute("asp-page", "/LoginPage");
    aElement.href = "/LoginPage";

    const savedCredentials = credentialsFromStorage();

    if (savedCredentials != null && savedCredentials.name !== null && savedCredentials.name !== undefined && savedCredentials.id !== null && savedCredentials.id !== undefined) {
        aElement.innerHTML = "Signed in as " + savedCredentials.name + ", shopping list id: " + savedCredentials.id + " <b>Logout</b>";
    } else {
        aElement.innerHTML = "Sign in";
    }

    aElement.addEventListener("click", () => {
        forgetCredentials();
        window.location.replace("/loginPage")
    });

    const navigationBarItemsUl = document.getElementById("navigationBarItemsUl");
    navigationBarItemsUl.appendChild(aElement);
});