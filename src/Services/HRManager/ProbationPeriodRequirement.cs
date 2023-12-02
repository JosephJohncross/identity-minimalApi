using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace src.Services.HRManager
{
    public class ProbationPeriodRequirement(int probationPeriod) : IAuthorizationRequirement
    {
        public int ProbationPeriod { get; set; } = probationPeriod;
    }

    public class ProbationPeriodRequirementHandler : AuthorizationHandler<ProbationPeriodRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProbationPeriodRequirement requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "EmployeeDate"))
            {
                return Task.CompletedTask;
            }

            if (DateTime.TryParse(context.User.FindFirst(x => x.Type == "EmployeeDate")?.Value, out DateTime result))
            {
                if ((DateTime.Now - result).Days > 30 * requirement.ProbationPeriod)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}