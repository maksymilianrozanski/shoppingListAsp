using System;
using System.ComponentModel.DataAnnotations;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Auth;
using ShoppingList.Data;
using ShoppingList.Dtos;

namespace ShoppingList.Pages
{
    public class ShoppingListCreate2 : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Shopping list name is required"),
         MaxLength(100)]
        public string ShoppingListName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Username is required"),
         MaxLength(100)]
        public string Username { get; set; }

        [BindProperty]
        [Required, MinLength(8), MaxLength(20)]
        public string Password { get; set; }

        private readonly BasicAuthenticationHandler _authenticationHandler;
        private readonly IShoppingListRepo _repository;

        public ShoppingListCreate2(BasicAuthenticationHandler authenticationHandler, IShoppingListRepo repository)
        {
            _authenticationHandler = authenticationHandler;
            _repository = repository;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (ModelState.IsValid)
                new ShoppingListCreateDto(ShoppingListName, Password).Pipe(createDto =>
                    _repository.CreateShoppingList(createDto)
                        .Map(readDto =>
                            _authenticationHandler
                                .CreateClaims(
                                    new BasicAuthenticationHandler.UserLoginData(readDto.Id, Username,
                                        createDto.Password))
                                .Map(c => HttpContext.SignInAsync("CookieAuthentication", c))
                                .Run()
                                .Match(l => throw new NotImplementedException(),
                                    r => Response.Redirect("/Protected/AddItems"))));
        }
    }
}