function credentialsFromStorage() {
    return JSON.parse(sessionStorage.getItem("credentials"));
}

function forgetCredentials() {
    sessionStorage.removeItem("credentials");
}

function authorizationHeaderValue() {
    const credentials = credentialsFromStorage();
    return "Basic " + window.btoa(credentials.name.toString() + ":" + credentials.password.toString());
}