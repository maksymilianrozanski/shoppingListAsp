using static LaYumba.Functional.F;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;

namespace ShoppingList.Pages
{
    public class Logout : PageModel
    {
        public bool IsSignedIn => ToOptionUser(HttpContext).Match(() => false, user => true);

        public Logout()
        {
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            HttpContext.SignOutAsync("CookieAuthentication");
            Response.Redirect("/Logout");
        }
    }
}