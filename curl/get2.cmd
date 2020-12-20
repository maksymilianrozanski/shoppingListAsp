rem requests shopping list from secured endpoint
curl -X GET --user user:password "localhost:5000/shoppingList/2" -v --ssl-no-revoke