using System;
using Microsoft.AspNetCore.Mvc;
using ShoppingList.Data;
using ShoppingList.Dtos;
using ShoppingList.Entities;

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
        public ActionResult<ShoppingListReadDto> Post(ShoppingListCreateDto listCreateDto)
        {
            Console.WriteLine("received post request");
            var shoppingList = listCreateDto;
            var created = _repository.CreateShoppingList(shoppingList);
            var savedSuccessfully = _repository.SaveChanges();

            if (savedSuccessfully)
                return CreatedAtAction(nameof(GetShoppingListById), new {created.Id},
                    (ShoppingListReadDto) created);
            else
                return NotFound();
        }
    }
}