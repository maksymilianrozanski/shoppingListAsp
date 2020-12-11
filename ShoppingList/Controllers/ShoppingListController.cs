using System;
using LaYumba.Functional;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using ShoppingList.Data;
using ShoppingList.Dtos;
using ShoppingList.Entities;
using static LaYumba.Functional.F;

namespace ShoppingList.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
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
            _repository.GetShoppingListEntityById(id).Match<ActionResult>(NotFound, Ok);

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("createList")]
        public ActionResult<ShoppingListReadDto> Post(ShoppingListCreateDto listCreateDto) =>
            _repository.CreateShoppingList(listCreateDto)
                .Match<ActionResult>(NotFound,
                    Some: i =>
                        CreatedAtAction(nameof(GetShoppingListById), new {i.Id},
                            i));

        [HttpPut]
        [Microsoft.AspNetCore.Mvc.Route("updateList")]
        public ActionResult Put(ShoppingListUpdateDto entity)
        {
            _repository.UpdateShoppingListEntity(entity);
            return NoContent();
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("addItem")]
        public ActionResult<ShoppingListReadDto> AddItemToTheList(ItemDataCreateDto itemDataCreateDto)
        {
            Console.WriteLine("addItem endpoint");
            return _repository.AddItemToShoppingList(itemDataCreateDto)
                .Match<ActionResult>(NotFound, Ok);
        }
    }
}