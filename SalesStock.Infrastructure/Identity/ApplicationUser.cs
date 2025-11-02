using Microsoft.AspNetCore.Identity;

namespace SalesStock.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public bool isActive { get; set; } = true;
    }
}