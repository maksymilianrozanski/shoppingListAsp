using System;
using System.Threading.Tasks;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static LaYumba.Functional.F;
using static ShoppingList.Auth.BasicAuthenticationHandler;
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