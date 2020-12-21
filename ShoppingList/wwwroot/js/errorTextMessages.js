function notSuccessfulResponseText(code, text, contentType) {
    if (code === 403) {
        return "Access denied, incorrect password?";
    } else if (code === 404) {
        return "Requested resource was not found";
    } else if (contentType === "text/plain; charset=utf-8") {
        if (code === 409) {
            switch (text) {
                case "ForbiddenOperation" :
                    return "Requested operation is not allowed";
                case "IncorrectUser" :
                    return "Current user cannot perform this operation";
                default:
                    return "Unknown error";
            }
        }
    } else return "Unknown error";
}

/**
 * @param {Response} unsuccessfulResponse
 * displays alert; Redirects to /loginPage when response status code 400, 401, 402, 403
 */
function handleFailure(unsuccessfulResponse) {
    if (unsuccessfulResponse.status >= 400 && unsuccessfulResponse.status < 404) {
        unsuccessfulResponse.text()
            .then(errorText => {
                    alert(notSuccessfulResponseText(unsuccessfulResponse.status, errorText, unsuccessfulResponse.headers.get("Content-Type")));
                    window.location.replace("/loginPage");
                }
            ).then(r => {
            return {
                successful: false,
                content: null
            }
        });
    } else {
        unsuccessfulResponse.text()
            .then(errorText =>
                alert(notSuccessfulResponseText(unsuccessfulResponse.status, errorText, unsuccessfulResponse.headers.get("Content-Type"))))
            .then(r => {
                return {
                    successful: false,
                    content: null
                }
            });
    }
}