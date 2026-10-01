using McDees.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace McDees.Web.Services;

public sealed class IdentitySeeder(RoleManager<IdentityRole> roles, UserManager<ApplicationUser> users, ApplicationDbContext db)
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
        await SeedInventoryAsync();
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

    private async Task SeedInventoryAsync()
    {
        if (await db.InventoryItems.AnyAsync()) return;
        db.InventoryItems.AddRange(
            Item("Beef patties", "Food", 180, "each", 75, .68m, "Walk-in freezer"),
            Item("Burger buns", "Food", 240, "each", 100, .22m, "Dry storage"),
            Item("American cheese", "Food", 420, "slices", 150, .12m, "Walk-in cooler"),
            Item("Bacon", "Food", 60, "lb", 18, 3.85m, "Walk-in cooler"),
            Item("Chicken fillets", "Food", 95, "each", 40, 1.04m, "Walk-in freezer"),
            Item("French fries", "Food", 112, "lb", 45, .91m, "Walk-in freezer"),
            Item("Onion rings", "Food", 34, "lb", 20, 1.28m, "Walk-in freezer"),
            Item("Lettuce", "Produce", 22, "heads", 10, 1.49m, "Walk-in cooler"),
            Item("Tomatoes", "Produce", 35, "lb", 15, 1.14m, "Walk-in cooler"),
            Item("Onions", "Produce", 47, "lb", 20, .56m, "Walk-in cooler"),
            Item("Pickles", "Food", 15, "jars", 8, 4.29m, "Walk-in cooler"),
            Item("Ketchup", "Condiments", 11, "bags", 8, 9.99m, "Dry storage"),
            Item("Soft drink syrup", "Beverages", 8, "boxes", 5, 78m, "Syrup rack"),
            Item("Shake mix", "Beverages", 19, "bags", 8, 16.25m, "Walk-in freezer"),
            Item("Small cups", "Packaging", 780, "each", 300, .06m, "Dry storage"),
            Item("Large cups", "Packaging", 620, "each", 300, .08m, "Dry storage"),
            Item("Cup lids", "Packaging", 1300, "each", 500, .03m, "Dry storage"),
            Item("Carry-out bags", "Packaging", 260, "each", 150, .11m, "Dry storage"),
            Item("Napkins", "Packaging", 4100, "each", 1500, .01m, "Dry storage"),
            Item("Fryer oil", "Supplies", 12, "jugs", 6, 22.50m, "Back room"),
            Item("Sanitizer", "Supplies", 5, "bottles", 4, 7.80m, "Back room"));
        await db.SaveChangesAsync();
    }

    private static InventoryItem Item(string name, string category, decimal onHand, string unit, decimal reorder, decimal cost, string location) =>
        new() { Name = name, Category = category, OnHandQuantity = onHand, Unit = unit, ReorderPoint = reorder, UnitCost = cost, StorageLocation = location };
}
