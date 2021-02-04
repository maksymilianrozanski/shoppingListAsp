using System;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FSharp.Core;
using SharedTypes.Dtos;
using ShoppingData;
using ShoppingList.Data.List;
using ShoppingList.Data.List.Errors;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;
using static LaYumba.Functional.F;
using ItemDataActionDto = SharedTypes.Dtos.Protected.ItemDataActionDto;

namespace ShoppingList.Controllers.Protected
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        private readonly IShoppingListRepo _repository;

        public ProtectedController(IShoppingListRepo repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Route("modifyItem")]
        public ActionResult<ShoppingListReadDto> ModifyShoppingListItem(ItemDataActionDto itemData) =>
            _repository.ModifyShoppingList(itemData)
                .Pipe(ShoppingListModificationResultTyped);

        [HttpGet("{id}")]
        public ActionResult<ShoppingListReadDto> GetShoppingListById(int id) =>
            ToOptionUser(HttpContext)
                .Bind(user => user.ShoppingListId == id ? Some(user.ShoppingListId) : new Option<int>())
                .Map(valid => _repository.GetShoppingListSorted(valid)
                    .Pipe(ShoppingListModificationResultTyped))
                .GetOrElse(NotFound());

        public ActionResult<ShoppingListReadDto> ShoppingListModificationResultTyped(
            Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> repositoryOperationResult)
            => repositoryOperationResult.Match<ActionResult>(
                l => l switch
                {
                    ShoppingListErrors.ShoppingListErrors.OtherError otherError => otherError.Item switch
                    {
                        SavingFailed => StatusCode(500),
                        ShopWaypointsNotFound => NotFound(l),
                        UnknownError => StatusCode(500),
                        _ => StatusCode(500)
                    },
                    var x when x.IsForbiddenOperation => Conflict(nameof(ShoppingListErrors.ShoppingListErrors
                        .ForbiddenOperation)),
                    var x when x.IsIncorrectPassword => StatusCode(403),
                    var x when x.IsIncorrectUser => Conflict(nameof(ShoppingListErrors.ShoppingListErrors
                        .IncorrectUser)),
                    var x when x.IsNotFound => NotFound(nameof(ShoppingListErrors.ShoppingListErrors
                        .NotFound)),
                    var x when x.IsItemWithIdAlreadyExists => Conflict(
                        nameof(ShoppingListErrors.ShoppingListErrors)),
                    //todo: add ShoppingListErrors.ShopNotFound handling
                    _ => StatusCode(500)
                }, Ok
            );
    }
}