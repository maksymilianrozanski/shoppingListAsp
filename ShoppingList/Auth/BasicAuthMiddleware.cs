using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ShoppingList.Auth
{
// -------------------------------------------------------------------------------------------------
// Based on https://johanbostrom.se/blog/adding-basic-auth-to-your-mvc-application-in-dotnet-core/
// Copyright (c) Johan Boström. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// -------------------------------------------------------------------------------------------------

    public class BasicAuthMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string realm;

        public BasicAuthMiddleware(RequestDelegate next, string realm)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the encoded username and password
                var encodedUsernamePassword =
                    authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();

                // Decode from Base64 to string
                var decodedUsernamePassword =
                    Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                // Split username and password
                var username = decodedUsernamePassword.Split(':', 2)[0];
                var password = decodedUsernamePassword.Split(':', 2)[1];

                // Check if login is correct
                if (CredentialsValidated(username, password))
                {
                    context.User = new GenericPrincipal(new GenericIdentity($"{username}:{password}"), null);
                    await next.Invoke(context);
                    return;
                }
                else
                {
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    return;
                }
            }

            // Return unauthorized
            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        }

        private bool CredentialsValidated(string username, string password) =>
            username.All(char.IsLetterOrDigit) && password.All(char.IsLetterOrDigit);
    }
}