using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ShoppingList.Auth
{
    public class BasicAuthenticationHandler
    {
        private readonly IUserService _userService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService)
        {
            _userService = userService;
        }


        public Try<ClaimsPrincipal> CreateClaims(User user) =>
            F.Try(async () =>
                    await _userService.Authenticate(user.Id, user.Username))
                .Map(i => i.Result)
                .Map(u =>
                    new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, u.Id),
                        new Claim(ClaimTypes.Name, u.Username)
                    })
                .Map(i => new ClaimsIdentity(i, "User Identity"))
                .Map(i => new ClaimsPrincipal(i));

        public interface IUserService
        {
            Task<User> Authenticate(string username, string password);
        }

        public class UserServiceImpl : IUserService
        {
            public Task<User> Authenticate(string username, string password) =>
                Task.FromResult(new User(username, password));
        }

        public class User
        {
            public User(string id, string username)
            {
                Id = id;
                Username = username;
            }

            public string Id { get; internal set; }
            public string Username { get; internal set; }
        }
    }
}