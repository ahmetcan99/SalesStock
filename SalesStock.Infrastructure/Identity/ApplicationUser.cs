using Microsoft.AspNetCore.Identity;

namespace SalesStock.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;
    }
}