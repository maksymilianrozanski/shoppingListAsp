using System;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using LaYumba.Functional;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using ShoppingData;
using ShoppingList.Data;
using ShoppingList.Dtos;
using ShoppingList.Entities;
using static LaYumba.Functional.F;
using static ShoppingList.Data.IShoppingListRepo;
using static ShoppingList.Data.IShoppingListRepo.RepoRequestError;

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
        public ActionResult<ShoppingListReadDto> GetShoppingListById(int id)
        {
            Console.Write("User's Identity?.Name");
            HttpContext.User.Identity?.Name.Pipe(Console.WriteLine);
            //todo: add verifying password
            return _repository.GetShoppingListEntityById(id).Match<ActionResult>(NotFound, Ok);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("addItem")]
        public ActionResult<ShoppingListReadDto> AddItemToTheList(ItemDataCreateDto itemDataCreateDto)
        {
            Console.WriteLine("addItem endpoint");
            return _repository.AddItemToShoppingList(itemDataCreateDto)
                .Pipe(ShoppingListModificationResult);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("modifyItem")]
        public ActionResult<ShoppingListReadDto> ModifyShoppingListItem(ItemDataActionDto itemDataActionDto)
        {
            Console.WriteLine("modifyItem endpoint");
            Console.WriteLine(itemDataActionDto.ToString());
            Console.WriteLine("action: " + itemDataActionDto.ActionNumber);
            return _repository.ModifyShoppingListItem(itemDataActionDto)
                .Pipe(ShoppingListModificationResult);
        }

        public ActionResult<ShoppingListReadDto> ShoppingListModificationResult(
            Either<string, ShoppingListReadDto> repositoryOperationResult) =>
            repositoryOperationResult.Match<ActionResult>(
                left => left switch
                {
                    nameof(ShoppingListErrors.ShoppingListErrors.ForbiddenOperation) => Conflict(left),
                    nameof(ShoppingListErrors.ShoppingListErrors.IncorrectPassword) => StatusCode(403),
                    nameof(ShoppingListErrors.ShoppingListErrors.IncorrectUser) => Conflict(left),
                    nameof(ShoppingListErrors.ShoppingListErrors.ListItemNotFound) => NotFound(left),
                    nameof(ShoppingListErrors.ShoppingListErrors.ItemWithIdAlreadyExists) => Conflict(left),
                    _ => StatusCode(500)
                }, Ok
            );
    }
}