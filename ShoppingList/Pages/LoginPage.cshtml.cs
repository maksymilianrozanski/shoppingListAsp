using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingData;
using ShoppingList.Auth;
using ShoppingList.Utils;
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

        public string ErrorMessage { get; set; } = "";

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
                .TryOptionEitherMap(
                    async c =>
                        await HttpContext.SignInAsync("CookieAuthentication", c))
                .TryOptionMap(j =>
                    j.Match(l =>
                        {
                            if (l.IsIncorrectPassword || l.IsNotFound)
                                this.ErrorMessage = "incorrect password or not found";
                            else
                                Response.Redirect("/Error");
                        },
                        _ =>
                            Response.Redirect("/")
                    )
                )
                .Run();
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