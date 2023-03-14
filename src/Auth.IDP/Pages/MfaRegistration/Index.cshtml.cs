using System.Net;
using System.Security.Cryptography;
using System.Text;
using Auth.IDP.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.IDP.Pages.MfaRegistration;

[SecurityHeaders]
[Authorize]
public class Index : PageModel
{
    private readonly ILocalUserService _localUserService;
    private readonly char[] _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    public ViewModel View { get; set; }
    
    [BindProperty]
    public InputModel Input { get; set; }
    
    private static string GetSecretUri(string provider, string email, string secret)
    {
        provider = WebUtility.UrlEncode(provider);
        email = WebUtility.UrlEncode(email);
        return $"otpauth://totp/{provider}:{email}?secret={secret}&issuer={provider}";
    }
    
    public Index(ILocalUserService localUserService)
    {
        _localUserService = localUserService;
    }

    public async Task OnGet()
    {
        var tokenData = RandomNumberGenerator.GetBytes(64);
        var result = new StringBuilder(16);
        for (int i = 0; i < 16; i++)  //todo: check if should include 16
        {
            var rnd = BitConverter.ToUInt32(tokenData, i * 4);
            var idx = rnd % _chars.Length;
            result.Append(_chars[idx]);
        }

        var secret = result.ToString();

        var subject = User.FindFirst(JwtClaimTypes.Subject)?.Value;
        var user = await _localUserService.GetUserBySubjectAsync(subject);

        var keyUri = GetSecretUri("Auth.IDP", user.Email, secret);

        View = new () { KeyUri = keyUri };
        Input = new() { Secret = secret };
    }
    
    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            var subject = User.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (await _localUserService.AddUserSecret(subject, "TOTP", Input.Secret))
            {
                await _localUserService.SaveChangesAsync();
                // return to the root 
                return Redirect("~/");
            }
            throw new Exception("MFA registration error");
        }
        return Page();
    }
}

public class ViewModel
{
    public string KeyUri { get; set; }
}

public class InputModel
{
    public string Secret { get; set; }
}