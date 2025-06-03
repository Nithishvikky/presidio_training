using Microsoft.AspNetCore.Authorization;

namespace ConsultingManagement.Authorization
{
    public class MinimumExperienceRequirement : IAuthorizationRequirement
    {
        public int Years { get; set; }
        public MinimumExperienceRequirement(int years)
        {
            Years = years;
        }
    }
}