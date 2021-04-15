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

requirements: docker engine, unused port 5000 To run the app in local environment, in directory containing
docker-compose.yml, enter in terminal: `docker-compose build`, and then `docker-compose up`. After displaying _Now
listening on: http://[::]:5000_ app should be available in the browser at `localhost:5000`. App is running without use
of https by default.

The format of JSON string inserted into ShopWaypointsReadDtoJSON column of ShopWaypointsEntities should match string
obtained by using `JsonSerializer.Serialize(ShoppingList.Data.Waypoints.WaypointsRepoHardcoded.HardcodedWaypoints)`.

To use different dataset for generating ML model, put training and test dataset into `./GroceryClassification/Data`
directory and adjust file names in the Main method of `./GroceryClassification/Program.cs`.

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
- Enable https in docker
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

**Instrukcja uruchomienia**

Wymagania:

- zainstalowany docker engine
- dostępny port 5000

(branch repozytorium wseiDemo)
W terminalu, w folderze gdzie znajduje się plik docker-compose.yml należy wprowadzić
`docker-compose build`

następnie `docker-compose up`

Po wyświetleniu _Now listening on: http://[::]:5000_ aplikacja powinna być dostępna w przeglądarce pod
adresem `localhost:5000`

Przy uruchomieniu z dockerem aplikacja działa bez https. W branchu wseiDemo podczas pierwszego uruchomienia dodawana
jest przykładowa mapa sklepu z nazwą _big-market_, oraz lista zakupowa z kilkoma dodanymi produktami.

Dane logowania:
Username: `user` Password: `password` dane wstawiane z pliku SqlShoppingListRepoExampleData.cs.
Dodatkowo aplikacja zwraca typ prognozowanego działu sklepowego (zapytania POST), przykładowe zapytanie w
pliku `/curl/predictItem.cmd`

W przypadku uruchamiania bez docker-a należy:

- dostosować w pliku `ShoppingList/appsettings.json` , tak aby odpowiadał dostępnej bazie danych (SQL Server).
- przed pierwszym uruchomieniem projektu ASP.NET uruchomić projekt GroceryClassification,
- przenieść wygenerowany plik model.zip do projektu ShoppingList
  `cp ./GroceryClassification/Models/model.zip ./ShoppingList/MLModels/model.zip`
- zaktualizować bazę danych ( `dotnet ef database update --project ShoppingList` ) - wymagana instalacja dotnet ef tools
- uruchomić projekt ShoppingList ( `dotnet run --project ShoppingList --urls http://*:5000` )
