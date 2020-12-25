using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShoppingList.Pages.Protected
{
    // [Authorize]
    public class ProtectedPage : PageModel
    {
        public void OnGet()
        {

        }
    }
}