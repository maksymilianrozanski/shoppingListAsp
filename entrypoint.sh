#!/bin/bash

set -e

>&2 echo "Starting GroceryClassification project"

until dotnet run --project GroceryClassification; do
>&2 echo "generating ml model"
sleep 1
done 

>&2 echo "copying model.zip"
cp ./GroceryClassification/Models/model.zip ./ShoppingList/MLModels/model.zip
>&2 echo "after copying model.zip"

run_cmd="dotnet run --project ShoppingList"

until dotnet ef database update --project ShoppingList; do
>&2 echo "SQL Server is starting up"
sleep 1
done

>&2 echo "SQL Server is up - executing command"
exec $run_cmd