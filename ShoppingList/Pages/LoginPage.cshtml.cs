using System;
using System.ComponentModel.DataAnnotations;
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

        [BindProperty] [Required] public string Username { get; set; } = "";

        [BindProperty] [Required] public string Password { get; set; } = "";

        [BindProperty] [Required] public int ShoppingListId { get; set; }

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
                //todo: display message when incorrect password
                .Match(_ => Response.Redirect("/LoginPage"),
                    _ => Response.Redirect("/"));
        }

        public void OnPost()
        {
            Console.WriteLine($"entered username: {Username}");
            Console.WriteLine($"entered shopping list id: {ShoppingListId}");
            if (ModelState.IsValid)
                SignIn();
        }
    }
}