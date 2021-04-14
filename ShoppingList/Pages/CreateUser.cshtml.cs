using System;
using System.ComponentModel.DataAnnotations;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedTypes.Dtos;
using ShoppingList.Auth;
using ShoppingList.Data.List;
using ShoppingList.Utils;

namespace ShoppingList.Pages
{
    public class CreateUser : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Username is required"),
         MaxLength(100)]
        public string Username { get; set; } = "";

        [BindProperty]
        [Required, MinLength(8), MaxLength(20)]
        public string Password { get; set; } = "";

        private readonly IShoppingListRepo _repository;
        private readonly IdBasedAuthenticationHandler _authenticationHandler;

        public string ErrorText { get; set; } = "";

        public CreateUser(IShoppingListRepo repository, IdBasedAuthenticationHandler authenticationHandler)
        {
            _repository = repository;
            _authenticationHandler = authenticationHandler;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (ModelState.IsValid)
                new UserCreateDto(Username, Password)
                    .Pipe(dto =>
                        _repository.CreateUser(dto))
                    .Bimap(l => this.ErrorText = l.ToString(),
                        r =>
                            _authenticationHandler.CreateClaims(new UserLoginData2(r.Login, r.Password))
                                .TryOptionEitherMap(c => HttpContext.SignInAsync("CookieAuthentication", c))
                                .Run()
                                .Match(_ => Response.Redirect("/Error"),
                                    _ => Response.Redirect("/Protected/ShoppingLists")));
        }
    }
}