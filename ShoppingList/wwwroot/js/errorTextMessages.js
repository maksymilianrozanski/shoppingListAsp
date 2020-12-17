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