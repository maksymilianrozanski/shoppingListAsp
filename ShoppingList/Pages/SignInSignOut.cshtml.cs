using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Auth;

namespace ShoppingList.Pages
{
    public class SignInSignOut : PageModel
    {
        public bool IsSignedIn => BasicAuthenticationHandler.IsSignedIn(HttpContext);

        public void OnGet()
        {
        }
    }
}