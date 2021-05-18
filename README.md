**App description**

App allows creating shopping lists and changing list item status. When user provides name of shop which is added to
database products, app displays products sorted in order which minimizes the distance.

In order to select the order providing the shortest route app predicts item type from item name. Predicting item type is
performed using model generated from dataset containing pairs of 'product type - product name'. Predicted item types are
matched with waypoints present in database (x,y coordinates mapping to location in specific shop where product type can
be found).

After selecting locations where products can be found items are sorted, app is searching for order where total distance
is the shortest. Selecting the shortest route is performed with use of "Google.OrTools"

ML model predicting item type is generated with use of dataset containing product names in Polish. To use product names
in different languages, different dataset should be used.

**Running the app**

requirements: docker engine, available port 443 and 80. To run the app in local environment, in directory containing
docker-compose.yaml, enter in terminal: `docker-compose build`, and then `docker-compose up`. After displaying
`info: Microsoft.Hosting.Lifetime[0]
Now listening on: https://[::]:443 info: Microsoft.Hosting.Lifetime[0]
Now listening on: http://[::]:80` app should be available in the browser at `localhost`.

The format of JSON string inserted into ShopWaypointsReadDtoJSON column of ShopWaypointsEntities should match string
obtained by using `JsonSerializer.Serialize(ShoppingList.Data.Waypoints.WaypointsRepoHardcoded.HardcodedWaypoints)`.

To use different dataset for generating ML model, put training and test dataset into `./GroceryClassification/Data`
directory and adjust file names in the Main method of `./GroceryClassification/Program.cs`.

Setting up certificate in Powershell ./shoppingList>

`dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\ShoppingList.pfx -p password_here`

Should return _The HTTPS developer certificate was generated successfully._

`dotnet user-secrets set "Kestrel:Certificates:Development:Password" "password_here"`

Should return _Successfully saved Kestrel:Certificates:Development:Password = password_here to the secret store._

**Example data**

`SqlShoppingListRepoExampleData.cs` injected in `ConfigureServices` method of `./ShoppingList/Startup.cs` inserts
example shop's waypoints and shopping list when ShoppingListEntities table is empty. To run the app without example
data, replace `services.AddTransient<IShoppingListRepo, SqlShoppingListRepoExampleData>();`

line in `./ShoppingList/Startup.cs` with

`services.AddTransient<IShoppingListRepo, SqlShoppingListRepo>();`

`SqlShoppingListRepoExampleData.cs` inserts shop named _big-market_ and example shopping list with id of _1_ and
password: _password_

**TODOs**

- Replace plain text passwords
- Remove dataset labels with too few entries

**Opis aplikacji**

Aplikacja pozwala na tworzenie listy zakupowej i oznaczania/zmiany statusu przedmiotów z listy. Jeżeli do listy zostanie
dodana nazwa sklepu, którego mapa/działy sklepowe znajdują się w bazie danych aplikacja wyznacza optymalną kolejność
zbierania produktów z listy, tak aby przebyta droga była jak najkrótsza (kolejność wyznaczana z użyciem biblioteki
Google.OrTools). Jeżeli w bazie danych znajduje się mapa sklepu, aplikacja prognozuje w jakim dziale sklepu
prawdopodobnie znajduje się produkt.

Prognoza odbywa się na podstawie modelu wygenerowanego z użyciem bazy nazw produktu powiązanych z nazwą produktu (model
wygenerowany z użyciem biblioteki Microsoft.ML). Baza danych produktów użyta do wygenerowania modelu:
https://www.kaggle.com/agatii/total-sale-2018-yearly-data-of-grocery-shop