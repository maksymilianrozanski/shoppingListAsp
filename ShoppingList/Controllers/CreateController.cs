using System;
using Microsoft.AspNetCore.Mvc;
using SharedTypes.Dtos;
using ShoppingList.Data.List;

namespace ShoppingList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CreateController : ControllerBase
    {
        private readonly IShoppingListRepo _repository;

        public CreateController(IShoppingListRepo repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Route("createList")]
        public ActionResult<ShoppingListReadDto> Post(ShoppingListCreateDto listCreateDto)
        {
            Console.WriteLine("create list endpoint");
            return _repository.CreateShoppingList(listCreateDto)
                .Match<ActionResult>(NotFound,
                    i =>
                        StatusCode(201, i));
        }
    }
}