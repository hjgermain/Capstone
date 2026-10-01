using McDees.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace McDees.Web.Services;

public sealed class IdentitySeeder(RoleManager<IdentityRole> roles, UserManager<ApplicationUser> users)
{
    // Development-only prototype accounts. Replace this approach before production.
    private const string DevelopmentPassword = "xyzzy123";
    private static readonly string[] RoleNames = ["Admin", "Customer", "Employee", "Manager", "Owner", "Corporate"];

    public async Task SeedAsync()
    {
        foreach (var role in RoleNames)
            if (!await roles.RoleExistsAsync(role)) await roles.CreateAsync(new IdentityRole(role));

        await CreateUserAsync("admin@mcdees.local", "McDees Administrator", "Admin", "Manager");
        await CreateUserAsync("manager@mcdees.local", "Morgan Manager", "Manager");
        await CreateUserAsync("employee@mcdees.local", "Casey Crew", "Employee");
        await CreateUserAsync("customer@mcdees.local", "Chris Customer", "Customer");
    }

    private async Task CreateUserAsync(string email, string name, params string[] userRoles)
    {
        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, DisplayName = name };
            var result = await users.CreateAsync(user, DevelopmentPassword);
            if (!result.Succeeded) throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
        foreach (var role in userRoles)
            if (!await users.IsInRoleAsync(user, role)) await users.AddToRoleAsync(user, role);
    }
}
