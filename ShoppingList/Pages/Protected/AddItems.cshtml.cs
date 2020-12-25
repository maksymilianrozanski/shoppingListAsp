using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Data;
using ShoppingList.Dtos;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;

namespace ShoppingList.Pages.Protected
{
    public class AddItems : PageModel
    {
        private readonly IShoppingListRepo _shoppingListRepo;

        [BindProperty] public string ItemName { get; set; }
        [BindProperty] public int Quantity { get; set; }
        [BindProperty] public Option<ShoppingListReadDto> ShoppingListReadDto { get; set; }

        public AddItems(IShoppingListRepo shoppingListRepo)
        {
            _shoppingListRepo = shoppingListRepo;
        }

        public void OnGet()
        {
            ShoppingListReadDto =
                ToOptionUser(HttpContext)
                    .Map(i => i.ShoppingListId)
                    .Bind(_shoppingListRepo.GetShoppingListEntityById);
        }
    }
}