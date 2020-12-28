using System;
using System.Security.Claims;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Auth;
using static ShoppingList.Auth.BasicAuthenticationHandler;
using static LaYumba.Functional.F;

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
                .ForEach(r =>
                {
                    Console.WriteLine(r);
                    if (r.IsCompletedSuccessfully)
                        Console.WriteLine("Signed in successfully");
                    else
                        Console.Write($"Signing in failed, {r.ToString()}");
                });
        }

        public void OnPost()
        {
            Console.WriteLine($"entered username: {Username}");
            Console.WriteLine($"entered password: {Password}");
            Console.WriteLine($"entered shopping list id: {ShoppingListId}");
            SignIn();
            Response.Redirect("/");
        }
    }
}