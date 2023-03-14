using Auth.Playground.CookieAuth.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Playground.CookieAuth.Pages
{
    [Authorize(Policy = Constants.AdminOnlyPolicyName)]
    public class SettingsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
