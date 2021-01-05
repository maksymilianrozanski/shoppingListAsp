rem requests predicting food type from item name
curl -X POST "localhost:5000/predict/predictItem" -d "@json/predictFoodType.json" -H "Content-Type: application/json" -v --ssl-no-revoke