using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace Auth.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource(
                name: "roles",
                displayName: "Your role(s)",
                userClaims: new[] { JwtClaimTypes.Role }),
            new IdentityResource(
                name: "country",
                displayName: "The country you are living in",
                userClaims: new[] { "country" })
        };

    // For access to more complex resources like a physical API
    // will be stored as an audience (aud) value in the token
    public static IEnumerable<ApiResource> ApiResources =>
        new[]
        {
            new ApiResource(
                name: "imagegalleryapi",
                displayName: "Image Gallery API",
                userClaims: new[] { "role", "country" })
            {
                Scopes =
                {
                    "imagegalleryapi.fullaccess",
                    "imagegalleryapi.read",
                    "imagegalleryapi.write"
                },
                ApiSecrets = { new Secret("apisecret".Sha256()) } // needed for introspection endpoint (reference token)
            }
        };

    // the scope of access requested for a client (ex. read, write) for more flexibility
    // it is stored in the scopes section in the token 
    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("imagegalleryapi.fullaccess"),
            new ApiScope("imagegalleryapi.read"),
            new ApiScope("imagegalleryapi.write"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client()
            {
                ClientName = "Image Gallery",
                ClientId = "imagegalleryclient",
                AllowedGrantTypes = GrantTypes.Code,
                AccessTokenType = AccessTokenType.Reference,
                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                RedirectUris =
                {
                    "https://localhost:7184/signin-oidc"
                },
                PostLogoutRedirectUris =
                {
                    "https://localhost:7184/signout-callback-oidc"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    // "imagegalleryapi.fullaccess",
                    "imagegalleryapi.read",
                    "imagegalleryapi.write",
                    "country"
                },
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                RequireConsent = false
            }
        };
}