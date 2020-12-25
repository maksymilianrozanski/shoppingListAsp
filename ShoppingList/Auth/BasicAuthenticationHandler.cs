using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static LaYumba.Functional.F;

namespace ShoppingList.Auth
{
    public class BasicAuthenticationHandler
    {
        private readonly IUserService<UserLoginData, User> _userService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService<UserLoginData, User> userService)
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
            public Task<User> Authenticate(UserLoginData t) =>
                Task.FromResult((User) t);
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

            public User(int shoppingListId, string username, string password)
            {
                ShoppingListId = shoppingListId;
                Username = username;
            }

            public static implicit operator User(UserLoginData u) =>
                new(u.ShoppingListId, u.Username, u.Password);
        }
    }
}