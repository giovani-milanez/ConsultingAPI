// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("consultingapi", "Consulting API")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "consultingapi" }
                },
                new Client
                {
                    ClientId = "swagger",
                    ClientName = "Swagger UI for Consulting API",
                    ClientSecrets = {new Secret("secret".Sha256())}, // change me!

                    // AllowedGrantTypes = new List<string> { "authorization_code" },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = false,
                    RequireClientSecret = false,
                    AllowOfflineAccess = true,

                    RedirectUris = {"https://localhost:44343/swagger/oauth2-redirect.html"},
                    AllowedCorsOrigins = {"https://localhost:44343"},
                    AllowedScopes = {"consultingapi", IdentityServerConstants.StandardScopes.OpenId , IdentityServerConstants.StandardScopes.Profile}
                }
            };
    }
}