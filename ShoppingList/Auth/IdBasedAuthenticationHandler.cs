using System.Security.Claims;
using LaYumba.Functional;
using Microsoft.AspNetCore.Http;
using SharedTypes.Dtos;
using ShoppingData;
using ShoppingList.Data;
using ShoppingList.Data.List;
using ShoppingList.Utils;
using static LaYumba.Functional.F;

namespace ShoppingList.Auth
{
    public class UserLoginData2
    {
        public string Username { get; }
        public string Password { get; }

        public UserLoginData2(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }

    public class IdBasedAuthenticationHandler
    {
        private readonly IUserService<UserLoginData2, User> _userService;

        public Try<Option<Either<ShoppingListErrors.ShoppingListErrors, ClaimsPrincipal>>> CreateClaims(
            UserLoginData2 user) =>
            Try(() => _userService.Authenticate(user))
                .TryOptionEitherMap(u =>
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, u.UserId.ToString()),
                        new Claim(ClaimTypes.Name, u.Username)
                    })
                .TryOptionEitherMap(j => new ClaimsIdentity(j, "User Identity"))
                .TryOptionEitherMap(claimsIdentity => new ClaimsPrincipal(claimsIdentity));

        public class UserServiceImpl : IUserService<UserLoginData2, User>
        {
            private readonly IShoppingListRepo _shoppingListRepo;

            public UserServiceImpl(IShoppingListRepo shoppingListRepo)
            {
                _shoppingListRepo = shoppingListRepo;
            }

            public Option<Either<ShoppingListErrors.ShoppingListErrors, User>>
                Authenticate(Option<UserLoginData2> user) =>
                user.Map(i => _shoppingListRepo.AuthenticateUser2(i).Map(dto => (User) dto));
        }

        public class User
        {
            public int UserId { get; }
            public string Username { get; }


            public User(int userId, string username)
            {
                UserId = userId;
                Username = username;
            }

            public static Option<User> ToOptionUser(Option<HttpContext> c) =>
                ExtractUserId(c)
                    .Bind(id => ExtractUsername(c)
                        .Map(username => (id, username)))
                    .Map(tuple => new User(int.Parse(tuple.id), tuple.username));

            private static Option<string> ExtractUserId(Option<HttpContext> context) =>
                context.Bind(c => c.User.Claims.Find(i =>
                        i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                    .Map(i => i.Value));

            private static Option<string> ExtractUsername(Option<HttpContext> context) =>
                context.Map(c => c.User.Identity).Bind(i => i != null ? Some(i.Name!) : None);

            public static implicit operator User(UserReadDto userReadDto) =>
                new(userReadDto.Id, userReadDto.Login);
        }

        public IdBasedAuthenticationHandler(IUserService<UserLoginData2, User> userService)
        {
            _userService = userService;
        }
        
        public static bool IsSignedIn(HttpContext context) =>
            User.ToOptionUser(context).Match(() => false, _ => true);
    }
}