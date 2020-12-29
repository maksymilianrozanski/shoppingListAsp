using System;
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
        [BindProperty] public string ShoppingListName { get; set; }

        [BindProperty] public string Username { get; set; }

        [BindProperty] public string Password { get; set; }

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
            new ShoppingListCreateDto(ShoppingListName, Password).Pipe(createDto =>
                _repository.CreateShoppingList(createDto)
                    .Map(readDto =>
                        _authenticationHandler
                            .CreateClaims(
                                new BasicAuthenticationHandler.UserLoginData(readDto.Id, Username, createDto.Password))
                            .Map(c => HttpContext.SignInAsync("CookieAuthentication", c))
                            .Run()
                            .Match(l => throw new NotImplementedException(),
                                r => Response.Redirect("/Protected/AddItems"))));
        }
    }
}