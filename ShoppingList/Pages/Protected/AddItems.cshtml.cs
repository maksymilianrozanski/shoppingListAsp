using System;
using LaYumba.Functional;
using LaYumba.Functional.Option;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using ShoppingList.Data;
using ShoppingList.Data.List;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;
using static LaYumba.Functional.F;
using static LaYumba.Functional.Exceptional;
using LaYumba.Functional.Option;
using ShoppingData;

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

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> DisplayedDto { get; set; }

        public void OnGet() =>
            DisplayedDto =
                ToOptionUser(HttpContext)
                    .Map(i => i.ShoppingListId)
                    .Map(_shoppingListRepo.GetShoppingList)
                    .GetOrElse(() => new Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>());

        public void OnPost()
        {
            ToOptionUser(HttpContext)
                .Map(i => new ItemDataCreateDto(i.ShoppingListId, ItemName, Quantity))
                .Map(i => _shoppingListRepo.AddItemToShoppingListDto(i))
                .ForEach(i => DisplayedDto = i);
        }
    }
}