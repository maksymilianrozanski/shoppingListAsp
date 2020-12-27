using System;
using System.Threading.Tasks;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Auth;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;

namespace ShoppingList.Pages
{
    public class Logout : PageModel
    {
        public void OnGet()
        {
        }

        public async Task OnPostAsync()
        {
            //todo: reload page after submitting post
            Console.WriteLine("sign out clicked");
            await HttpContext.SignOutAsync("CookieAuthentication");
        }
    }
}