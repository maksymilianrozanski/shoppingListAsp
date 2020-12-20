/**
 * @param {string} id - id of shopping list to fetch
 * @return response of /shoppingList/{id} endpoint
 */
async function fetchShoppingList(id) {
    return Promise.resolve(
        await fetch("/shoppingList/" + id, {
            method: 'GET',
            mode: 'cors', // no-cors, *cors, same-origin
            cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
            credentials: 'same-origin', // include, *same-origin, omit
            headers: {
                'Accept': 'application/json'
            },
            redirect: 'follow',
            referrerPolicy: 'no-referrer',
        }).then(r => {
            if (r.status === 200) {
                console.log("fetching shopping list data successful");
                return r.json();
            } else {
                console.log("fetching shopping list data not successful, response status: " + r.status);
            }
        })
    );
}