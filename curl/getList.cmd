rem requests shopping list from secured endpoint
curl -X POST "localhost:5000/shoppingList/listById" -d "@json/getList.json" -H "Content-Type: application/json" -v --ssl-no-revoke