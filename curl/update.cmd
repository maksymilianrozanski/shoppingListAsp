rem requests updating existing list
curl -X PUT "localhost:5000/shoppingList" -d "@json/updateList.json" -H "Content-Type: application/json" -v --ssl-no-revoke