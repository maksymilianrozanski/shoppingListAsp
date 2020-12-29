using System;
using System.Security.Claims;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Auth;
using static ShoppingList.Auth.BasicAuthenticationHandler;
using static LaYumba.Functional.F;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;

namespace ShoppingList.Pages
{
    public class LoginPage : PageModel
    {
        private readonly BasicAuthenticationHandler _authenticationHandler;

        [BindProperty] public string Username { get; set; }

        [BindProperty] public string Password { get; set; }

        [BindProperty] public int ShoppingListId { get; set; }

        public void OnGet()
        {
        }

        public LoginPage(BasicAuthenticationHandler authenticationHandler)
        {
            _authenticationHandler = authenticationHandler;
        }

        private void SignIn()
        {
            _authenticationHandler.CreateClaims(new UserLoginData(ShoppingListId, Username, Password))
                .Map(c => HttpContext.SignInAsync("CookieAuthentication", c))
                .Run()
                .Match(l => Response.Redirect("/LoginPage"),
                    r => Response.Redirect("/"));
        }

        public void OnPost()
        {
            Console.WriteLine($"entered username: {Username}");
            //todo: remove logging password
            Console.WriteLine($"entered password: {Password}");
            Console.WriteLine($"entered shopping list id: {ShoppingListId}");
            SignIn();
        }
    }
}