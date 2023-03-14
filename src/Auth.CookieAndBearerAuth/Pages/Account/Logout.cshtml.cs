using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.CookieAndBearerAuth.Pages.Account;

public class Logout : PageModel
{
    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync("JWT_OR_COOKIE");
        return RedirectToPage("/index");
    }
}