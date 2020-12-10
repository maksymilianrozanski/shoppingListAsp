using System;
using LaYumba.Functional;
using Microsoft.AspNetCore.Mvc;
using ShoppingList.Data;
using ShoppingList.Dtos;
using ShoppingList.Entities;
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
        public ActionResult<ShoppingListReadDto> GetShoppingListById(int id) =>
            _repository.GetShoppingListEntityById(id).Match<ActionResult>(NotFound,
                i => Ok((ShoppingListReadDto) i));

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