using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShoppingList.Pages
{
    public class LoginPage2 : PageModel
    {
        [BindProperty] public string Username { get; set; }

        [BindProperty] public string Password { get; set; } = "";


        public void OnGet()
        {
        }

        public void OnPost()
        {
            Console.WriteLine($"entered username: {Username}");
            RedirectToPage("/protected/protectedPage");
        }
    }
}