Autor: Maksymilian Różański

Link do repozytorium (branch wseiDemo):
https://github.com/maksymilianrozanski/shoppingListAsp/tree/wseiDemo

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
Username: dowolny Password: `password` Shopping list id: `1` dane wstawiane z pliku SqlShoppingListRepoExampleData.cs.
Dodatkowo aplikacja zwraca typ prognozowanego działu sklepowego (zapytania POST), przykładowe zapytanie w
pliku `/curl/predictItem.cmd`

W przypadku uruchamiania bez docker-a należy:
- dostosować w pliku `ShoppingList/appsettings.json` , tak aby odpowiadał dostępnej bazie danych (SQL Server).
- przed pierwszym uruchomieniem projektu ASP.NET uruchomić projekt GroceryClassification,
- przenieść wygenerowany plik model.zip do projektu ShoppingList
  `cp ./GroceryClassification/Models/model.zip ./ShoppingList/MLModels/model.zip`
- zaktualizować bazę danych ( `dotnet ef database update --project ShoppingList` ) - wymagana instalacja dotnet ef tools
- uruchomić projekt ShoppingList ( `dotnet run --project ShoppingList --urls http://*:5000` )
