using Database.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace API.Extension
{
    public static class HttpContextExtensions
    {
        public static User GetLoggedInUser(this HttpContext @this)
        {            
            var user = @this.Items["LoggedInUser"] as User;
            if (user == null)
            {
                var authToken = @this.Request.Headers["Authorization"].FirstOrDefault() ?? @this.Request.Query["access_token"];

                if (String.IsNullOrWhiteSpace(authToken))
                {
                    return null;
                }

                // basic auth shouldnt continue
                if (authToken.StartsWith("Basic "))
                    return null;

                // additional info
                if (authToken.Contains("Bearer "))
                    authToken = authToken.GetAfter("Bearer ");

                // parse token
                var token = new JwtSecurityToken(authToken);
                user = token.ExtractUser();

                @this.Items["LoggedInUser"] = user;
            }
            return user;
        }
    }
}
