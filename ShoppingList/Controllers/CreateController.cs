using System;
using Microsoft.AspNetCore.Mvc;
using ShoppingList.Data;
using ShoppingList.Dtos;

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

        [HttpGet]
        public ActionResult<string> GetGreetings() => Ok("Welcome!");

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("createList")]
        public ActionResult<ShoppingListReadDto> Post(ShoppingListCreateDto listCreateDto)
        {
            Console.WriteLine("create list endpoint");
            Console.WriteLine(listCreateDto.Name);
            return _repository.CreateShoppingList(listCreateDto)
                .Match<ActionResult>(NotFound,
                    Some: i =>
                        StatusCode(201, i));
        }
    }
}