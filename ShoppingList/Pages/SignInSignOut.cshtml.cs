using Microsoft.AspNetCore.Mvc.RazorPages;
using static ShoppingList.Pages.Logout;

namespace ShoppingList.Pages
{
    public class SignInSignOut : PageModel
    {
        public bool IsSignedIn => IsSignedInFunc(HttpContext);

        public void OnGet()
        {
        }
    }
}