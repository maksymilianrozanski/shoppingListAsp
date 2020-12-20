rem requests shopping list from secured endpoint
curl -X GET --user user:winter "localhost:5000/shoppingList/2"  -H -v --ssl-no-revoke