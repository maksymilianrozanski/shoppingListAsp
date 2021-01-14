using System;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FSharp.Core;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
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
                .Bind(valid => _repository.GetShoppingListReadDtoByIdWithSorting(valid))
                .Map<ShoppingListReadDto, ActionResult>(Ok)
                .GetOrElse(NotFound());

        public ActionResult<ShoppingListReadDto> ShoppingListModificationResultTyped(
            Either<Error, ShoppingListReadDto> repositoryOperationResult)
            => repositoryOperationResult.Match<ActionResult>(
                l => l switch
                {
                    SavingFailed savingFailed => StatusCode(500),
                    ShopWaypointsNotFound shopWaypointsNotFound => NotFound(l),
                    UnknownError unknownError => StatusCode(500),
                    OtherError otherError => otherError.ErrorObject switch
                    {
                        var x when x.IsForbiddenOperation => Conflict(nameof(ShoppingListErrors.ShoppingListErrors
                            .ForbiddenOperation)),
                        var x when x.IsIncorrectPassword => StatusCode(403),
                        var x when x.IsIncorrectUser => Conflict(nameof(ShoppingListErrors.ShoppingListErrors
                            .IncorrectUser)),
                        var x when x.IsListItemNotFound => NotFound(nameof(ShoppingListErrors.ShoppingListErrors
                            .ListItemNotFound)),
                        var x when x.IsItemWithIdAlreadyExists => Conflict(
                            nameof(ShoppingListErrors.ShoppingListErrors)),
                        _ => throw new MatchFailureException()
                    },
                    _ => throw new ArgumentOutOfRangeException(nameof(l)),
                }, Ok
            );
    }
}