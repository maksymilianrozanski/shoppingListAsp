using System;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Auth;
using ShoppingList.Data;
using ShoppingList.Dtos;
using ShoppingList.Dtos.Protected;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;
using static LaYumba.Functional.F;

namespace ShoppingList.Pages.Protected
{
    public class AddItems : PageModel
    {
        private readonly IShoppingListRepo _shoppingListRepo;

        [BindProperty] public string ItemName { get; set; }
        [BindProperty] public int Quantity { get; set; }
        public Option<ShoppingListReadDto> ShoppingListReadDto { get; set; }

        public AddItems(IShoppingListRepo shoppingListRepo)
        {
            _shoppingListRepo = shoppingListRepo;
            ShoppingListReadDto = (Option<ShoppingListReadDto>) null!;
        }

        public void OnGet()
        {
            ShoppingListReadDto =
                ToOptionUser(HttpContext)
                    .Map(i => i.ShoppingListId)
                    .Bind(_shoppingListRepo.GetShoppingListEntityById);
        }

        public void OnPost()
        {
            var newItem = ToOptionUser(HttpContext)
                .Map(i => new ItemDataCreateDtoNoPassword(i.ShoppingListId, ItemName, Quantity))
                .Map(i => _shoppingListRepo.AddItemToShoppingListNoPassword(i));

            newItem.ForEach(i =>
                i.Match(left => Response.Redirect("/Error"),
                    right => ShoppingListReadDto = right));
        }
    }
}