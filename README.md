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
- Send new kiosk orders to the role-protected Kitchen Production Display, where employees advance tickets from New to Being Prepared, Ready, and Completed.
- Show the kiosk’s current New, Being Prepared, and Ready orders in a live lower-right order board; kitchen updates reach it immediately through SignalR.
- Let managers view a prototype-wide inventory endpoint with on-hand amounts, reorder thresholds, values, and low-stock flags.

The menu and order services are deliberately separated from MVC pages so they can next be backed by PostgreSQL, ASP.NET Core Identity, and the kitchen production display.

## Prototype sign-in and database

The application uses PostgreSQL and ASP.NET Core Identity without migrations during rapid prototyping. On startup it calls `EnsureCreated` and seeds these development accounts, all with password `xyzzy123`:

| Account | Role | Landing page |
| --- | --- | --- |
| `customer@mcdees.local` | Customer | Customer kiosk |
| `employee@mcdees.local` | Employee | Employee workspace |
| `manager@mcdees.local` | Manager | Restaurant tools |
| `admin@mcdees.local` | Admin, Manager | Restaurant tools + database reset |

The **Restaurant tools** page contains an Admin-only control to destroy and rebuild the prototype database. Type `REBUILD` in the confirmation field before it will execute. It immediately restores the development roles and accounts, then signs the administrator out.

The required PostgreSQL connection string is read from .NET user-secrets as `ConnectionStrings:DefaultConnection`; it is deliberately not stored in this repository.

## Operations routes

- `/Kitchen` — available to Employee, Manager, and Admin accounts. Kiosk orders live in memory for the current application session.
- `/Inventory` — available to Manager, Admin, Owner, and Corporate accounts. The prototype seeds 21 inventory items when the inventory table is empty.

## Prototype limits

Orders and the menu are currently in memory. Restarting the application clears them. No payment, authentication, inventory, or kitchen queue persistence has been added yet.
