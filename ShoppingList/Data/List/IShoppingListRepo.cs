using System;
using System.Collections.Generic;
using LaYumba.Functional;
using SharedTypes.Dtos;
using SharedTypes.Dtos.Protected;
using ShoppingData;
using ShoppingList.Auth;
using static ShoppingList.Auth.IdBasedAuthenticationHandler;
using ItemDataActionDto = SharedTypes.Dtos.Protected.ItemDataActionDto;

namespace ShoppingList.Data.List
{
    public interface IShoppingListRepo
    {
        public Either<ShoppingListErrors.ShoppingListErrors, UserCreatedReadDto> CreateUser(
            Option<UserCreateDto> userCreateDto);

        public Either<ShoppingListErrors.ShoppingListErrors, bool> UserExists(Option<string> login);

        public Either<ShoppingListErrors.ShoppingListErrors, UserReadDto> FindUserById(int userId);

        public Either<ShoppingListErrors.ShoppingListErrors, UserReadDto> FindUserByLogin(string login);

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> CreateShoppingList(
            Option<ShoppingListCreateDto> shoppingList);

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> CreateShoppingList2(
            Option<ShoppingListWithUserCreateDto> shoppingList);

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>
            GetShoppingList(int id);

        public Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto>
            GetShoppingListSorted(int id);

        public Either<ShoppingData.ShoppingListErrors.ShoppingListErrors, IEnumerable<ShoppingListReadDto>>
            GetUserShoppingLists(string userLogin);

        Either<ShoppingListErrors.ShoppingListErrors, int> PasswordMatchesShoppingList(int shoppingListId,
            string password);

        Either<ShoppingData.ShoppingListErrors.ShoppingListErrors, UserReadDto> AuthenticateUser2(
            UserLoginData2 userLoginData);

        Either<ShoppingData.ShoppingListErrors.ShoppingListErrors, (User, int)> ValidateAccess(
            (User, int) userWithListId);

        Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> AddItemToShoppingListDto(
            Option<ItemDataCreateDto> itemToAdd);

        Either<ShoppingListErrors.ShoppingListErrors, ShoppingListReadDto> ModifyShoppingList(
            Option<ItemDataActionDto> itemDataAction);

        bool SaveChanges();
    }
}