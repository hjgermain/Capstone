using Microsoft.AspNetCore.Identity;

namespace McDees.Web.Data;

public sealed class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}
