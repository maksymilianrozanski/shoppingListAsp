rem requests adding new shopping list
curl -X POST "localhost:5000/shoppingList/createList" -d "@json/shoppingList.json" -H "Content-Type: application/json" -v --ssl-no-revoke
