using McDees.Web.Models;

namespace McDees.Web.Services;

public sealed class MenuCatalog
{
    private static readonly IReadOnlyList<ModifierOption> BurgerModifiers =
    [new(1, "No onions", 0), new(2, "Extra pickles", 0), new(3, "Add cheese", .75m), new(4, "Add bacon", 1.25m)];

    public IReadOnlyList<MenuItem> Items { get; } =
    [
        new(1, "Classic Burger", "Burgers", "Seasoned beef, lettuce, tomato, pickles, onions, and house sauce.", 5.49m, "🍔", "Contains gluten", BurgerModifiers),
        new(2, "Double Burger", "Burgers", "Two beef patties, American cheese, pickles, and onions.", 7.29m, "🍔", "Contains dairy & gluten", BurgerModifiers),
        new(3, "Crispy Chicken", "Chicken", "Crispy chicken, shredded lettuce, pickles, and mayo.", 6.19m, "🍗", "Contains gluten", [new(1, "No pickles", 0), new(2, "Add cheese", .75m)]),
        new(4, "Garden Veggie Burger", "Burgers", "Plant-based patty with lettuce, tomato, and pickles.", 6.49m, "🥬", "Vegetarian; contains gluten", BurgerModifiers),
        new(5, "Golden Fries", "Sides", "Crisp, salted fries made fresh for every order.", 2.79m, "🍟", "Vegetarian", [new(1, "No salt", 0), new(2, "Add cheese sauce", .75m)]),
        new(6, "Onion Rings", "Sides", "Crispy battered onion rings with dipping sauce.", 3.29m, "🧅", "Contains gluten", [new(1, "Extra sauce", .35m)]),
        new(7, "Vanilla Shake", "Desserts", "Hand-spun vanilla shake.", 3.99m, "🥤", "Contains dairy", [new(1, "Add whipped cream", 0)]),
        new(8, "Fountain Drink", "Drinks", "Choose a chilled soft drink at pickup.", 2.49m, "🥤", "", [new(1, "No ice", 0), new(2, "Large", .60m)])
    ];

    public MenuItem? Find(int id) => Items.SingleOrDefault(item => item.Id == id);
}
