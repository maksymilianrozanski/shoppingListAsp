using System;
using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc;
using ShoppingList.Data;
using ShoppingList.Dtos;
using static LaYumba.Functional.F;

namespace ShoppingList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShoppingListController : ControllerBase
    {
        private readonly IShoppingListRepo _repository;

        public ShoppingListController(IShoppingListRepo repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}", Name = "GetShoppingListById")]
        public ActionResult<ShoppingListReadDto> GetShoppingListById(int id)
        {
            var item = _repository.GetShoppingListEntityById(id);
            Console.WriteLine($"item received from database: ${item}");
            if (item != null)
                return Ok(item);
            else
                return NotFound();
        }

        [HttpPost]
        public ActionResult<ShoppingListReadDto> Post(ShoppingListCreateDto listCreateDto) =>
            _repository.CreateShoppingList(listCreateDto)
                .Bind(i => _repository.SaveChanges() ? Some(i) : null)
                .Match<ActionResult>(NotFound,
                    Some: i =>
                        CreatedAtAction(nameof(GetShoppingListById), new {i.Id},
                            (ShoppingListReadDto) i));
    }
}