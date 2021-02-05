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
    public class CreateList : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Username is required"),
         MaxLength(100)]
        public string Username { get; set; } = "";

        [BindProperty]
        [Required, MinLength(8), MaxLength(20)]
        public string Password { get; set; } = "";

        [BindProperty] [MaxLength(100)] public string? ShopName { get; set; } = "";

        private readonly BasicAuthenticationHandler _authenticationHandler;
        private readonly IShoppingListRepo _repository;

        public string ErrorText { get; set; } = "";

        public CreateList(BasicAuthenticationHandler authenticationHandler, IShoppingListRepo repository)
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
                new ShoppingListCreateDto(Password, ShopName ?? "")
                    .Pipe(createDto =>
                        _repository.CreateShoppingList(createDto)
                            .Bimap(l =>
                                    l.Pipe(i => this.ErrorText = i.ToString()),
                                readDto => _authenticationHandler
                                    .CreateClaims(
                                        new BasicAuthenticationHandler.UserLoginData(readDto.Id, Username,
                                            createDto.Password))
                                    .TryOptionEitherMap(c =>
                                        HttpContext.SignInAsync("CookieAuthentication", c))
                                    .Run()
                                    .Match(_ => Response.Redirect("/Error"),
                                        _ => Response.Redirect("/Protected/AddItems"))));
        }
    }
}