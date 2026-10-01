# McDees

McDees is a restaurant operations prototype. The current milestone is a customer self-order kiosk built with ASP.NET Core MVC.

## Run the prototype

1. Install the .NET 10 SDK.
2. From the repository root, run `dotnet run --project McDees.Web`.
3. Open the local address shown in the terminal (normally `http://localhost:5000`).

## What works now

- Browse a starter burger-restaurant menu by category.
- Customize menu items with selected modifiers.
- Add, increase, decrease, and remove cart items; the cart remains available during the browser session.
- Review an order with estimated tax and a simulated pay-at-pickup checkout.
- Receive a generated order number and pickup code.

The menu and order services are deliberately separated from MVC pages so they can next be backed by PostgreSQL, ASP.NET Core Identity, and the kitchen production display.

## Prototype limits

Orders and the menu are currently in memory. Restarting the application clears them. No payment, authentication, inventory, or kitchen queue persistence has been added yet.
