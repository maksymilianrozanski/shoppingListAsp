using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedTypes.Dtos;
using ShoppingList.Auth;
using ShoppingList.Data.List;
using ShoppingList.Utils;

namespace ShoppingList.Pages.Protected
{
    public class ShoppingLists : PageModel
    {
        private readonly IShoppingListRepo _shoppingListRepo;

        [BindProperty]
        public IEnumerable<ShoppingListReadDto> ShoppingListReadDtos { get; set; } = new List<ShoppingListReadDto>();

        public ShoppingLists(IShoppingListRepo shoppingListRepo)
        {
            _shoppingListRepo = shoppingListRepo;
        }

        public Option<IdBasedAuthenticationHandler.User> CurrentUser { get; set; } = new None();
        [BindProperty] [MaxLength(100)] public string? ShopName { get; set; } = "";
        public string ErrorText { get; set; } = "";

        public void OnGet()
        {
            this.CurrentUser = IdBasedAuthenticationHandler.User.ToOptionUser(HttpContext);

            this.CurrentUser.Map(user =>
                //todo: add error handling
                _shoppingListRepo.GetUserShoppingLists(user.Username)
                    .ForEach(i => this.ShoppingListReadDtos = i));
        }

        public RedirectToPageResult OnPostOpen(int shoppingListId)
        {
            return RedirectToPage("/Protected/AddItems", "OpenList", new {shoppingListId = shoppingListId});
        }

        public void OnPostCreate()
        {
            if (ModelState.IsValid)
                IdBasedAuthenticationHandler.User.ToOptionUser(HttpContext).Map(user =>
                    new ShoppingListWithUserCreateDto(user.UserId, ShopName ?? "")
                        .Pipe(createDto =>
                            _shoppingListRepo.CreateShoppingList2(createDto)
                                .Match(l =>
                                        this.ErrorText = l.ToString(),
                                    _ => Response.Redirect("/Protected/ShoppingLists"))));
        }
    }
}