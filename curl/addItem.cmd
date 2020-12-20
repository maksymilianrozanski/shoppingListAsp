rem requests adding new item to shopping list
curl -X POST --user user:winter "localhost:5000/shoppingList/addItem" -d "@json/addItem2.json" -H "Content-Type: application/json" -v --ssl-no-revoke
