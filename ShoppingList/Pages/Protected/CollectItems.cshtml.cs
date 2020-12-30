using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Data;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;

namespace ShoppingList.Pages.Protected
{
    public class CollectItems : PageModel
    {
        public CollectItems(IShoppingListRepo shoppingListRepo)
        {
            RefreshUserData();
        }

        public string CurrentUsername { get; private set; } = "";
        public int CurrentShoppingListId { get; private set; }

        public void OnGet() => RefreshUserData();

        private void RefreshUserData() =>
            ToOptionUser(HttpContext).ForEach(i =>
            {
                CurrentUsername = i.Username;
                CurrentShoppingListId = i.ShoppingListId;
            });
    }
}