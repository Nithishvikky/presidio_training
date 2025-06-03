using System.Security.Claims;
using ConsultingManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ConsultingManagement.Authorization
{
    public class ExperienceHandler : AuthorizationHandler<MinimumExperienceRequirement>
    {
        private readonly IDoctorService _doctorService;

        public ExperienceHandler(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumExperienceRequirement requirement)
        {
            var userName = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userName == null) return;

            var doctor = await _doctorService.GetDoctByEmail(userName);

            if (doctor == null) return;

            if (doctor.YearsOfExperience >= requirement.Years)
            {
                context.Succeed(requirement);
            }
        }
    }
}