using Microsoft.AspNetCore.Authorization;

namespace Auth.Playground.CookieAuth.Authorization;

public class HrManagerProbationRequirement : IAuthorizationRequirement
{
    public int ProbationMonths { get; }

    public HrManagerProbationRequirement(int probationMonths)
    {
        ProbationMonths = probationMonths;
    }
}

public class HrManagerProbationRequirementHandler : AuthorizationHandler<HrManagerProbationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HrManagerProbationRequirement requirement)
    {
        if (context.User.HasClaim(x => x.Type.Equals("HiringDate")))
        {
            var hiringDate = DateTime.Parse(
                context.User.FindFirst(x => x.Type.Equals("HiringDate"))?.Value!);
            var period = DateTime.Now - hiringDate;
            if (period.Days > 30 * requirement.ProbationMonths)
            {
                context.Succeed(requirement);
            }
        }
        return Task.CompletedTask;
    }
}