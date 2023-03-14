using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.IDP.Pages.User.ActivationCodeSent;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    public void OnGet()
    {
        
    }
}