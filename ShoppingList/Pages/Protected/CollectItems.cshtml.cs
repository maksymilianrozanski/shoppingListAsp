using System;
using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Auth;
using ShoppingList.Data;
using ShoppingList.Data.List;

namespace ShoppingList.Pages.Protected
{
    public class CollectItems : PageModel
    {
        public CollectItems(IShoppingListRepo shoppingListRepo)
        {
        }

        public string CurrentUsername { get; private set; } = "";
        public int CurrentShoppingListId { get; private set; }

        public void OnGetCollect(int shoppingListId)
        {
            this.CurrentShoppingListId = shoppingListId;
            IdBasedAuthenticationHandler.User.ToOptionUser(HttpContext).ForEach(i => { CurrentUsername = i.Username; });
        }
    }
}