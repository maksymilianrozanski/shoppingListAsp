using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Data;

namespace ShoppingList.Pages.Protected
{
    public class AddItems : PageModel
    {
        private readonly IShoppingListRepo _shoppingListRepo;

        [BindProperty] public string ItemName { get; set; }
        [BindProperty] public int Quantity { get; set; }

        public AddItems(IShoppingListRepo shoppingListRepo)
        {
            _shoppingListRepo = shoppingListRepo;
        }

        public void OnGet()
        {
        }
    }
}