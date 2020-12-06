using Microsoft.AspNetCore.Mvc;
using ShoppingList.Dtos;
using ShoppingData;

namespace ShoppingList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShoppingListController : ControllerBase
    {
        [HttpPost]
        public ActionResult<ShoppingListReadDto> Post(ShoppingListCreateDto listCreateDto)
        {
            var emptyList = ShoppingListModule.emptyShoppingList(listCreateDto.Name, 4, listCreateDto.Password);
            return Ok(emptyList);
        }
    }
}