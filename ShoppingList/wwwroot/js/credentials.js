function credentialsFromStorage() {
    return JSON.parse(sessionStorage.getItem("credentials"));
}

function saveCredentials(credentials) {
    sessionStorage.setItem("credentials", JSON.stringify(credentials));
}

function forgetCredentials() {
    sessionStorage.removeItem("credentials");
}

function authorizationHeaderValue() {
    const credentials = credentialsFromStorage();
    return "Basic " + window.btoa(credentials.name.toString() + ":" + credentials.password.toString());
}