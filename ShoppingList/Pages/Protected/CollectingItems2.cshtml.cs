using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Data;
using ShoppingList.Dtos;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;

namespace ShoppingList.Pages.Protected
{
    public class CollectingItems2 : PageModel
    {
        private readonly IShoppingListRepo _shoppingListRepo;
        
        public string CurrentUsername { get; private set; }
        public int CurrentShoppingListId { get; private set; }

        public CollectingItems2(IShoppingListRepo shoppingListRepo)
        {
            _shoppingListRepo = shoppingListRepo;
        }

        public void OnGet()
        {
            ToOptionUser(HttpContext).ForEach(i =>
            {
                CurrentUsername = i.Username;
                CurrentShoppingListId = i.ShoppingListId;
            });
        }
    }
}