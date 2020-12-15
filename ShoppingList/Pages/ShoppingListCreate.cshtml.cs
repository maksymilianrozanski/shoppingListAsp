using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingList.Dtos;

namespace ShoppingList.Pages
{
    public class ShoppingListCreateModel : PageModel
    {
        
        [BindProperty]
        public ShoppingListCreateDto ShoppingListCreateDto { get; set; }
        
        public void OnGet()
        {
            
        }
    }
}