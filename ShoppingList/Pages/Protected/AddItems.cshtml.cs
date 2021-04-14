using System;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using ShoppingList.Data;
using ShoppingList.Data.List;
using static LaYumba.Functional.F;
using static LaYumba.Functional.Exceptional;
using LaYumba.Functional.Option;
using ShoppingData;
using ShoppingList.Auth;

namespace ShoppingList.Pages.Protected
{
    public class AddItems : PageModel
    {
        private readonly IShoppingListRepo _shoppingListRepo;

        public AddItems(IShoppingListRepo shoppingListRepo)
        {
            _shoppingListRepo = shoppingListRepo;
        }

        [BindProperty] public string ItemName { get; set; } = "";
        [BindProperty] public int Quantity { get; set; }

        [BindProperty] public int CurrentShoppingListId { get; set; } = -1;

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> DisplayedDto { get; set; }

        public void OnGetOpenList(int shoppingListId)
        {
            this.CurrentShoppingListId = shoppingListId;
            DisplayedDto = _shoppingListRepo.GetShoppingList(this.CurrentShoppingListId);
        }

        public RedirectToPageResult OnPostAddItem(int shoppingListId)
        {
            IdBasedAuthenticationHandler.User.ToOptionUser(HttpContext)
                .Map(i => new ItemDataCreateDto(shoppingListId, ItemName, Quantity))
                .Map(i => _shoppingListRepo.AddItemToShoppingListDto(i))
                .ForEach(i => DisplayedDto = i);

            return RedirectToPage("/Protected/AddItems", "OpenList",
                new {shoppingListId = shoppingListId});
        }

        public RedirectToPageResult OnPostOpenCollecting(int shoppingListId)
        {
            return RedirectToPage("/Protected/CollectItems", "Collect", new {shoppingListId = shoppingListId});
        }
    }
}