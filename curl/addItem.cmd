rem requests adding new item to shopping list
curl -X POST "localhost:5000/shoppingList/addItem" -d "@json/addItem.json" -H "Content-Type: application/json" -v --ssl-no-revoke