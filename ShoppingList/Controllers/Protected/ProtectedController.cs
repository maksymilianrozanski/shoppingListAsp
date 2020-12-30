using LaYumba.Functional;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingData;
using ShoppingList.Data;
using ShoppingList.Dtos;
using ShoppingList.Dtos.Protected;
using static ShoppingList.Auth.BasicAuthenticationHandler.User;
using static LaYumba.Functional.F;

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
        public ActionResult<ShoppingListReadDto> ModifyShoppingListItem(ItemDataActionDtoNoPassword itemData) =>
            _repository.ModifyShoppingListItemNoPassword(itemData)
                .Pipe(ShoppingListModificationResult);

        [HttpGet("{id}")]
        public ActionResult<ShoppingListReadDto> GetShoppingListById(int id) =>
            ToOptionUser(HttpContext)
                .Bind(user => user.ShoppingListId == id ? Some(user.ShoppingListId) : new Option<int>())
                .Bind(valid => _repository.GetShoppingListEntityById(valid))
                .Map<ShoppingListReadDto, ActionResult>(Ok)
                .GetOrElse(NotFound());

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