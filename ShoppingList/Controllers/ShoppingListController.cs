using System;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using LaYumba.Functional;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
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
        public ActionResult<ShoppingListReadDto> GetShoppingListById(int id) =>
            ParseIdentity(HttpContext).Map(i => new ShoppingListGetRequest(id, i.Item2))
                .Map(i => _repository.GetShoppingListEntityByIdIfPassword(i))
                .Map(r =>
                    r.Match<ActionResult>(left =>
                        left switch
                        {
                            IncorrectPassword => StatusCode(403),
                            RepoRequestError.NotFound => NotFound(),
                            _ => throw new ArgumentOutOfRangeException(nameof(left), left, null)
                        }, Ok)).GetOrElse(NotFound);

        private static Option<(string, string)> ParseIdentity(HttpContext context) =>
            (Option<(string, string)>) context.User.Identity?.Name?
                .Pipe(i =>
                    i != null
                        ? Some((i.Split(':', 2)[0], i.Split(':', 2)[1]))
                        : None)!;

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