rem requests adding new shopping list
curl -X POST "https://localhost:5001/shoppinglist" -d "@json/shoppingList.json" -v -H "Content-Type: application/json" --ssl-no-revoke
