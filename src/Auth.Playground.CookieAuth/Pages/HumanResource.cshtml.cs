using Auth.Playground.CookieAuth.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Playground.CookieAuth.Pages
{
    [Authorize(Policy = Constants.MustBelongHrPolicyName)]
    public class HumanResourceModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
