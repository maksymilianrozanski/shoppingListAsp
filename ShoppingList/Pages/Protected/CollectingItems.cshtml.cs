using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Data;
using ShoppingList.Dtos;
using static LaYumba.Functional.F;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;

namespace ShoppingList.Pages.Protected
{
    public class CollectingItems : PageModel
    {
        
        private readonly IShoppingListRepo _shoppingListRepo;

        public Option<ShoppingListReadDto> ShoppingListReadDto { get; set; }

        public CollectingItems(IShoppingListRepo shoppingListRepo)
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