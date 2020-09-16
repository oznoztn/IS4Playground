using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RawCodingAuth.Basics.Auth.CustomAuthorizationRequirements
{
    public class RequireExperienceClaim : IAuthorizationRequirement
    {
        public int ExperiencePoints { get; }

        public RequireExperienceClaim(int experiencePoints)
        {
            ExperiencePoints = experiencePoints;
        }
    }

    /// <summary>
    /// Eğer secretGarden:xp claim'i varsa, claim değerinin belirtilen değerden büyük olması koşulunu arar
    /// </summary>
    public class RequireExperiencePointClaimHandler : AuthorizationHandler<RequireExperienceClaim>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireExperienceClaim requirement)
        {
            const string claimType = "secretGarden:xp";
             
            var claim = context.User.Claims.ToList().FirstOrDefault(t => t.Type == claimType);

            if (claim != null)
            {
                var experiencePoints = int.Parse(claim.Value);

                if (experiencePoints >= requirement.ExperiencePoints)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

            return Task.CompletedTask;
        }
    }
}
