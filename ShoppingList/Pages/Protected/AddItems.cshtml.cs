using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using ShoppingList.Data;
using ShoppingList.Data.List;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;
using static LaYumba.Functional.F;

namespace ShoppingList.Pages.Protected
{
    public class AddItems : PageModel
    {
        private readonly IShoppingListRepo _shoppingListRepo;

        public AddItems(IShoppingListRepo shoppingListRepo)
        {
            _shoppingListRepo = shoppingListRepo;
            ShoppingListReadDto = null!;
        }

        [BindProperty] public string ItemName { get; set; } = "";
        [BindProperty] public int Quantity { get; set; }
        public Option<ShoppingListReadDto> ShoppingListReadDto { get; set; }

        public void OnGet() =>
            ShoppingListReadDto =
                ToOptionUser(HttpContext)
                    .Map(i => i.ShoppingListId)
                    .Bind(_shoppingListRepo.GetShoppingListReadDtoById);

        public void OnPost()
        {
            var newItem = ToOptionUser(HttpContext)
                .Map(i => new ItemDataCreateDto(i.ShoppingListId, ItemName, Quantity))
                .Map(i => _shoppingListRepo.AddItemToShoppingListDto(i));

            newItem.ForEach(i =>
                i.Match(_ => Response.Redirect("/Error"),
                    right => ShoppingListReadDto = right));
        }
    }
}