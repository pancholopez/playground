using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImageGallery.Client.Controllers;

[Authorize]
public class AuthenticationController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthenticationController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // todo: change this to POST
    // GET
    public async Task Logout()
    {
        var client = _httpClientFactory.CreateClient("IDPClient");

        var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync();
        if (discoveryDocumentResponse.IsError)
        {
            throw new Exception(discoveryDocumentResponse.Error);
        }

        var accessRevocationResponse = await client.RevokeTokenAsync(
            new TokenRevocationRequest
            {
                Address = discoveryDocumentResponse.RevocationEndpoint,
                ClientId = "imagegalleryclient",
                ClientSecret = "secret",
                Token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken)
            });

        if (accessRevocationResponse.IsError)
        {
            throw new Exception(accessRevocationResponse.Error);
        }
        
        var refreshTokenRevocationResponse = await client.RevokeTokenAsync(
            new TokenRevocationRequest
            {
                Address = discoveryDocumentResponse.RevocationEndpoint,
                ClientId = "imagegalleryclient",
                ClientSecret = "secret",
                Token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken)
            });
        
        if (refreshTokenRevocationResponse.IsError)
        {
            throw new Exception(refreshTokenRevocationResponse.Error);
        }

        // logout client app (this client)
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // redirect to IDP to end session cookie
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}