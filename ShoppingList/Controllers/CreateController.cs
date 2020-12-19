using Microsoft.AspNetCore.Mvc;
using ShoppingList.Data;

namespace ShoppingList.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public class CreateController : ControllerBase
    {
        private readonly IShoppingListRepo _repository;

        public CreateController(IShoppingListRepo repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult<string> GetGreetings() => Ok("Welcome!");
    }
}