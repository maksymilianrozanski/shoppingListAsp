using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using LaYumba.Functional;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ShoppingList.Auth
{
    public class BasicAuthenticationHandler 
        // : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService)
            // : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }

        // private Try<AuthenticateResult> ResultFromHeader() =>
        //     F.Try(() =>
        //             (AuthenticationHeaderValue.Parse(Request.Headers["Authorization"])))
        //         .Map(i => i.Parameter)
        //         .Map(Convert.FromBase64String)
        //         .Map(p => Encoding.UTF8.GetString(p).Split(new[] {':'}, 2))
        //         .Map(i => (i[0], i[1]))
        //         .Map(async tuple => await _userService.Authenticate(tuple.Item1, tuple.Item2))
        //         .Map(i => i.Result)
        //         .Map(user =>
        //             new[]
        //             {
        //                 new Claim(ClaimTypes.NameIdentifier, user.Id),
        //                 new Claim(ClaimTypes.Name, user.Username)
        //             })
        //         .Map(i => new ClaimsIdentity(i, Scheme.Name))
        //         .Map(i => new ClaimsPrincipal(i))
        //         .Map(i => new AuthenticationTicket(i, Scheme.Name))
        //         .Map(AuthenticateResult.Success);

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
                .Map(i =>
                {
                    Console.WriteLine("before");
                    var claims = new ClaimsIdentity(i, "User Identity");
                    Console.WriteLine("after");
                    return claims;
                })
                .Map(i => new ClaimsPrincipal(i));

        // protected override async Task<AuthenticateResult> HandleAuthenticateAsync() =>
        //     ResultFromHeader().Run()
        //         .Match(exception =>
        //         {
        //             Console.WriteLine("HandleAuthenticateAsync failed");
        //             return AuthenticateResult.Fail("Authentication failed");
        //         }, success =>
        //         {
        //             Console.WriteLine("HandleAuthenticateAsync success");
        //             return success;
        //         });

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