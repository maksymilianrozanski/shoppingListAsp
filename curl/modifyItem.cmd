rem requests modifying existing shopping list item
curl -X POST "localhost:5000/shoppingList/modifyItem" -d "@json/modifyItem.json" -H "Content-Type: application/json" -v --ssl-no-revoke