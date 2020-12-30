using System.Security.Claims;
using System.Threading.Tasks;
using LaYumba.Functional;
using Microsoft.AspNetCore.Http;
using ShoppingList.Data;
using static LaYumba.Functional.F;

namespace ShoppingList.Auth
{
    public class BasicAuthenticationHandler
    {
        private readonly IUserService<UserLoginData, User> _userService;

        public BasicAuthenticationHandler(IUserService<UserLoginData, User> userService)
        {
            _userService = userService;
        }

        public Try<ClaimsPrincipal> CreateClaims(UserLoginData user) =>
            Try(async () =>
                    await _userService.Authenticate(user))
                .Map(i => i.Result)
                .Map(u =>
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, u.ShoppingListId.ToString()),
                        new Claim(ClaimTypes.Name, u.Username)
                    })
                .Map(i => new ClaimsIdentity(i, "User Identity"))
                .Map(i => new ClaimsPrincipal(i));

        public interface IUserService<in T, TR>
        {
            Task<TR> Authenticate(T t);
        }

        public class UserServiceImpl : IUserService<UserLoginData, User>
        {
            private readonly IShoppingListRepo _shoppingListRepo;

            public UserServiceImpl(IShoppingListRepo shoppingListRepo)
            {
                _shoppingListRepo = shoppingListRepo;
            }

            public Task<User> Authenticate(UserLoginData t) =>
                (Task<User>) _shoppingListRepo.PasswordMatchesShoppingList(t.ShoppingListId, t.Password)
                    .Match(_ => Task.FromException(null),
                        _ => Task.FromResult((User) t));
        }

        public class UserLoginData
        {
            public int ShoppingListId { get; }
            public string Username { get; }
            public string Password { get; }

            public UserLoginData(int shoppingListId, string username, string password)
            {
                ShoppingListId = shoppingListId;
                Username = username;
                Password = password;
            }
        }

        public class User
        {
            public int ShoppingListId { get; }
            public string Username { get; }

            public User(int shoppingListId, string username)
            {
                ShoppingListId = shoppingListId;
                Username = username;
            }

            public static implicit operator User(UserLoginData u) =>
                new(u.ShoppingListId, u.Username);

            public static Option<User> ToOptionUser(Option<HttpContext> c) =>
                ExtractShoppingListId(c)
                    .Bind(id => ExtractUsername(c)
                        .Map(username => (id, username)))
                    .Map(tuple => new User(int.Parse(tuple.id), tuple.username));

            private static Option<string> ExtractShoppingListId(Option<HttpContext> context) =>
                context.Bind(c => c.User.Claims.Find(i =>
                        i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                    .Map(i => i.Value));

            private static Option<string> ExtractUsername(Option<HttpContext> context) =>
                context.Map(c => c.User.Identity).Bind(i => i != null ? Some(i.Name!) : None);
        }

        public static bool IsSignedIn(HttpContext context) =>
            User.ToOptionUser(context).Match(() => false, _ => true);
    }
}