// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using IdentityModel;
using System.Security.Claims;
using System.Text.Json;
using Duende.IdentityServer;
using Duende.IdentityServer.Test;

namespace Auth.IDP;

public class TestUsers
{
    public static List<TestUser> Users
    {
        get
        {
            var address = new
            {
                street_address = "One Hacker Way",
                locality = "Heidelberg",
                postal_code = 69118,
                country = "Germany"
            };
                
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "Emma",
                    Password = "password",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Role, "PayingUser"),
                        new Claim(JwtClaimTypes.Name, "Emma Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Emma"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "EmmaSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "https://EmmaSmith.com"),
                        new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("country", "be"),
                    }
                },
                new TestUser
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "David",
                    Password = "password",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Role, "FreeUser"),
                        new Claim(JwtClaimTypes.Name, "David Smith"),
                        new Claim(JwtClaimTypes.GivenName, "David"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "DavidSmith@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "https://DavidSmith.com"),
                        new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("country", "nl"),
                    }
                }
            };
        }
    }
}