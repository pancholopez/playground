using Auth.IDP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.IDP.Pages.User.Activation;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly ILocalUserService _localUserService;

    [BindProperty] public InputModel Input { get; set; } = new InputModel();

    public Index(ILocalUserService localUserService)
    {
        _localUserService = localUserService;
    }
    
    public async Task<IActionResult> OnGet(string securityCode)
    {
        if (await _localUserService.ActivateUserAsync(securityCode))
        {
            Input.Message = "Account activated! You can log in now.";
        }
        else
        {
            Input.Message = "Your account is NOT active. Contact your administrator.";
        }

        await _localUserService.SaveChangesAsync();

        return Page();
    }
}