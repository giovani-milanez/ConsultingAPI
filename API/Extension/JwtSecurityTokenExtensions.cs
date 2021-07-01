using Database.Extension;
using Database.Model;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace API.Extension
{
    public static class JwtSecurityTokenExtensions
    {
        /// <summary>
        /// Extracts an user instance based on the claims we have
        /// </summary>
        public static User ExtractUser(this JwtSecurityToken @this)
        {
            User user = new User();
            // create the instance based on the claims
            foreach (var claim in @this.Claims)
            {
                switch (claim.Type)
                {
                    case "sub": user.Id = Convert.ToInt32(claim.Value); break;
                    case "name": user.Name = claim.Value; break;
                    case "email": user.Email = claim.Value; break;
                    case "role": user.Type = claim.Value; break;
                    default:
                        break;
                }
            }

            return user;
        }
    }
}