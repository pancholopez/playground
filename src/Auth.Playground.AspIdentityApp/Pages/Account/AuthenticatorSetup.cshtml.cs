using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Auth.Playground.AspIdentityApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

namespace Auth.Playground.AspIdentityApp.Pages.Account;

[Authorize]
public class AuthenticatorSetup : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty] public AuthenticatorSetupViewModel ViewModel { get; set; } = new();

    [BindProperty] public bool Succeeded { get; set; }
    
    public AuthenticatorSetup(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (key == null)
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            key = await _userManager.GetAuthenticatorKeyAsync(user);
        }
        
        ViewModel.Key = key;
        ViewModel.QrCodeBytes = GenerateQrCodeBytes("MyAuthApp", key, user.Email);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var user = await _userManager.GetUserAsync(User);
        var tfaSuccess = await _userManager.VerifyTwoFactorTokenAsync(
            user: user, 
            tokenProvider: TokenOptions.DefaultAuthenticatorProvider,
            token: ViewModel.SecurityCode);

        if (tfaSuccess)
        {
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            Succeeded = true;
        }
        else
        {
            ModelState.AddModelError("AuthenticatorSetup","Authenticator setup failed.");
        }
        return Page();
    }

    private byte[] GenerateQrCodeBytes(string provider, string key, string userEmail)
    {
        var qrCodeGenerator = new QRCodeGenerator();
        var qrCodeData = qrCodeGenerator.CreateQrCode(
            $"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}",
            QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }
}

public class AuthenticatorSetupViewModel
{
    public string Key { get; set; }
    
    [Required]
    [DisplayName("Security Code")]
    public string SecurityCode { get; set; }
    
    public byte[] QrCodeBytes { get; set; }
}