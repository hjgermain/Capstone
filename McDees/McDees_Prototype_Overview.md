# McDees Restaurant Operations Platform

**Prepared for:** H. James “Jim” De St Germain  
**Project repository:** `github.com/hjgermain/Capstone/McDees`  
**Technology:** C#, ASP.NET Core MVC, ASP.NET Core Identity, PostgreSQL

## Purpose

McDees is a restaurant operations application that begins with the two screens that make a quick-service hamburger restaurant run: a customer self-order kiosk and a live kitchen production display. From that foundation, it grows into a single integrated system for menu administration, inventory, receiving, employee scheduling/timekeeping, reporting, and multi-store corporate oversight.

The first prototype should feel like a real restaurant that has been open for months: it should launch with an active menu, real-looking customers and employees, current orders in several preparation states, staff currently clocked in, schedules, stock levels, historical orders, and recent inventory activity. The application initially operates one store, but every operational record should be scoped to a store so expansion to additional locations does not require a database redesign.

## 1. Product Overview and Initial Priorities

### 1.1 Primary prototype experience

1. A customer browses a visual hamburger menu, customizes an item, and submits an anonymous or signed-in order.
2. The order immediately appears on the **Kitchen Production Display (KPD)**, a staff-facing screen showing what must be made and when.
3. Kitchen staff advance orders through preparation states until they are ready and completed.
4. Managers can maintain menu items, prices, images, ingredients, employees, and core restaurant data.

### 1.2 Later expansion areas

- Employee schedules, time clock, attendance, payroll/salary data, and labor reporting.
- Ingredient-level inventory depletion when food is prepared or sold.
- Deliveries/receiving, including importing or scanning a supplier packing list.
- Customer account history, loyalty points, rewards, feedback, refunds, and promotions.
- Store comparisons and corporate reporting across locations.

### 1.3 Design principles

- Treat the kiosk as a low-friction, touch-friendly public experience; keep staff/management pages protected.
- Preserve historical facts: changing a menu price or item name must not alter a prior order.
- Use simple, explicit workflow states rather than deleting records.
- Enforce authorization on the server, not merely by hiding buttons.
- Keep the data model store-aware from day one (`StoreId` on store-owned records).

## 2. Software Tools and Development Process

### 2.1 Core stack

| Area | Initial choice |
|---|---|
| Web application | C# / ASP.NET Core MVC with Razor Views |
| Authentication and roles | ASP.NET Core Identity |
| Data access | Entity Framework Core with Npgsql |
| Database | PostgreSQL |
| Source control | Git and GitHub |
| Repository | `hjgermain/Capstone/McDees` |
| Image storage (prototype) | Local application-managed uploads; later object/blob storage |
| Automated checks | Unit tests for services/workflows; integration tests for key MVC endpoints |

### 2.2 Recommended project organization

- `McDees.Web` — MVC UI, controllers, views, view models, authentication configuration.
- `McDees.Data` — EF Core entities, `ApplicationDbContext`, migrations, seeding support.
- `McDees.Services` — order workflow, price calculation, inventory consumption, reporting logic.
- `McDees.Tests` — service and integration tests.
- `seed/restaurant-seed.json` — deterministic realistic demo data (source data, not production truth).

Keep entities separate from view models. Controllers should coordinate requests; business rules such as totals, order-status transitions, inventory deductions, and role checks belong in services.

### 2.3 Git process

- Use small feature branches and pull requests where practical.
- Commit migrations with the entity/model changes that require them.
- Never commit secrets, database passwords, API keys, or production connection strings; use user secrets and environment variables.
- Include setup instructions in `README.md`: PostgreSQL setup, migrations, seed command, and default development accounts.

## 3. Roles and Authorization

| Role | Main permissions |
|---|---|
| Anonymous kiosk customer | Browse menu, configure items, submit order, view an order confirmation using a non-guessable pickup token. |
| Customer | Anonymous permissions plus order history, profile, and later loyalty/rewards. |
| Employee | View the kitchen display as assigned; clock in/out; see own schedule and time records. |
| Manager | Employee permissions plus manage orders, menu availability/pricing, inventory, schedules, and store reports. |
| Owner | Manager permissions plus broader financial, employee compensation, and owner reports for owned stores. |
| Corporate | Cross-store administration, standard menu/catalog, store-level dashboards, and corporate reporting. |

An account may need more than one role. Store assignment is separate from role: for example, an employee can be a manager at Store A without gaining management access at every future store.

## 4. Customer Self-Order Kiosk (First Build Target)

### 4.1 Customer flow

1. Kiosk opens to a visual category menu: Burgers, Chicken, Meals, Sides, Drinks, Desserts, and Kids.
2. Customer selects an item and sees price, image, description, dietary/allergen information, and customization options.
3. Customer adds items to a persistent cart, reviews quantities, customizations, subtotal/tax/total, and order notes.
4. Customer chooses **Continue as guest** or signs in; payment is initially a simulated/recorded tender rather than a live payment gateway.
5. Submission creates an order, displays an order number/pickup code, and sends the order to the KPD.

### 4.2 Menu and customization rules

- A menu item belongs to one or more categories and can be active, temporarily unavailable, or archived.
- A meal is a composition: main item + eligible side + eligible drink, with price rules for upgrades.
- Modifiers support choices such as *no onions*, *extra pickles*, cheese choices, sauce choices, and add-ons with price differences.
- Keep a **snapshot** of each ordered item: displayed name, unit price, tax, selected modifiers, and modifier prices. Do not depend on today’s menu record to recreate yesterday’s receipt.
- Model allergens and dietary tags (e.g., contains dairy, gluten-aware—not “gluten free” unless verified) so they can be displayed and reported.

### 4.3 Initial menu content

Seed a recognizable hamburger-restaurant menu, including: Classic Burger, Double Burger, Bacon Cheeseburger, Chicken Sandwich, Crispy Chicken, Veggie Burger, nuggets, fries, onion rings, salads, shakes, soft drinks, kids meals, and 6–10 defined meals/combo options. Include item images or stable placeholder image paths.

## 5. Kitchen Production Display (KPD) (First Build Target)

The KPD is the employee-facing operational screen—better named than “employee kiosk”—that answers: **What should we cook now?** It should be optimized for quick scanning on a shared monitor/tablet.

### 5.1 Required behavior

- Show new, acknowledged, in-preparation, ready, and completed orders in separate visual groupings or filtered views.
- Make order age prominent; highlight orders exceeding a configurable service target.
- Show item details and modifiers clearly, including special instructions and allergy flags.
- Allow authorized kitchen staff to transition status: `New → Acknowledged → Preparing → Ready → Completed`; allow manager-controlled cancellation/refund states.
- Use server-side refresh/polling first; later replace or supplement with SignalR for immediate updates.
- Record status-history timestamps and the employee who performed each transition when available.

### 5.2 Order model and workflow

`Order` contains store, order number, customer/guest identity, channel, timestamps, tender/payment status, totals, pickup token, and overall status. `OrderLine` contains the historical item snapshot and quantity. `OrderLineModifier` stores the chosen customizations. `OrderStatusHistory` provides the auditable timeline.

Initial order channels: `Kiosk`, `Counter`, and `ManagerEntered`; future channels can include mobile, delivery, and drive-through.

## 6. Menu, Catalog, Pricing, and Media Administration

Managers need protected CRUD pages for categories, menu items, meals, modifiers, prices, availability, nutritional/allergen data, and images.

- Support effective date ranges for prices and promotions rather than overwriting prices without history.
- Allow an image upload, alt text, ordering/display priority, and a fallback placeholder image.
- Ingredient recipes should define the ingredients and expected quantities used by each menu item/modifier.
- Temporarily mark an item unavailable if it is out of stock; do not delete it from historical records.
- Corporate may eventually maintain a standard catalog while a store can have approved local pricing/availability overrides.

## 7. Inventory, Recipes, and Receiving

### 7.1 Inventory foundations

Track inventory per store and per inventory item: units, on-hand quantity, reorder point, target level, unit cost, supplier SKU, storage location, lot/expiration when relevant, and active status. Examples include beef patties, buns, lettuce, cheese, fries, cups, lids, napkins, and cleaning supplies.

Use inventory transactions rather than only changing a number: `OpeningBalance`, `DeliveryReceived`, `RecipeConsumption`, `Waste`, `ManualAdjustment`, `Transfer`, and `StockCount`.

### 7.2 Sales consumption

Each menu item/modifier has a recipe (ingredient usages). When an order reaches the defined fulfillment point—initially `Completed`—create ingredient consumption transactions. This makes the MVP understandable and avoids prematurely handling waste from cancelled orders. Later, consume at `Preparing` and create reversal/waste flows as needed.

### 7.3 Receiving shipments (later prototype stage)

Create a supplier delivery/packing-list workflow: supplier, expected lines, delivered quantity, accepted/rejected quantity, substitutions, invoice number, received-by employee, date/time, and item costs. Start with manual entry or JSON/CSV import. A barcode scanner can later behave as keyboard input to match supplier SKU/barcode lines and create `DeliveryReceived` transactions.

## 8. Employees, Scheduling, Time Clock, and Compensation

### 8.1 Employee records

Each employee links to an Identity user and maintains employee number, contact information, store assignments, employment status, job roles/positions, hire date, hourly/salary pay configuration, and optional emergency/contact data. Sensitive compensation data must be restricted to manager/owner/corporate roles as appropriate.

### 8.2 Scheduling and timekeeping

- `Shift`: store, position, scheduled start/end, break expectations, assigned employee, and status.
- `TimeClockEntry`: clock-in/out timestamps, source, approved/edited flags, edit reason, and manager approval.
- Employees can see their own schedule and clock themselves in/out; managers handle corrections.
- A shared staff time-clock page can use an employee PIN/badge only as a prototype convenience. Do not treat it as strong authentication; require secure authentication for sensitive tasks.
- Reports should compare scheduled versus actual hours, late arrival, missed clock-outs, overtime thresholds, and labor cost.

## 9. Customer Accounts, Service, and Loyalty

Identity customers should have profile data and a customer record. Seed customers include `jim@eathere.com` and additional realistic accounts such as `customer1@eathere.com` through `customer49@eathere.com` (correcting the intended domain consistently to `eathere.com`).

Build order history first. Later add loyalty accounts, point accrual rules, rewards/redemptions, customer preferences, saved favorite orders, receipts, customer support notes, refunds, and satisfaction feedback.

## 10. Store and Corporate Readiness

Seed one store but create a `Store` entity now: store code, name, address, phone, time zone, tax configuration, hours, status, and default service targets. All store-scoped tables must reference it: orders, employees/assignments, schedules, inventory, receiving, local menu overrides, and reports.

Corporate features can begin as read-only rollups: sales, order volume, average ticket, preparation time, inventory exceptions, labor percentage, and store comparisons. Keep global/reference data (standard ingredients and catalog templates) distinct from store operational data.

## 11. Seed Data: Full-Swing Restaurant Demo Requirement

### 11.1 Recommended implementation

Use a version-controlled **large JSON seed file** for readable, deliberate demo content plus a C# `DatabaseSeeder` service that imports it. This is preferable to embedding a large data graph in EF Core `HasData`, which is awkward for identity users, related historical data, passwords, and changing sample dates.

At application startup in the **Development** environment, run migrations and call the seeder idempotently. The seeder should use stable external seed keys, check whether the McDees demo store already exists, and either skip or offer an explicit development-only reset/reseed command. Never automatically reseed/delete production data.

### 11.2 Seed dataset contents

| Area | Minimum realistic seed scope |
|---|---|
| Stores | 1 active store, with structure ready for more. |
| Identity/roles | All six roles and default demo accounts for manager, owner, corporate, employees, and 50 customers. |
| Customers | 50 total: `jim@eathere.com` plus `customer1`–`customer49`; varied names, creation dates, and histories. |
| Employees | 18–25 employees across manager, shift lead, cook, cashier, prep, and maintenance positions. |
| Current staff | 6–10 currently clocked in, with current shifts and a few active breaks. |
| Schedules/time clock | 8–12 weeks of schedules and time entries, including realistic late/missed-clock-out exceptions. |
| Menu/catalog | Categories, 25–35 menu items, meals, recipes, modifiers, price history, and image paths. |
| Inventory | 35–60 ingredients/supplies with realistic levels, costs, reorder points, and recent movements. |
| Receiving | 6–10 historic deliveries plus one expected/recent delivery. |
| Orders | 3–6 months of historical orders; 12–20 current-day orders including 5–8 active KPD orders at different statuses. |
| Operational exceptions | At least one low-stock item, unavailable menu item, cancelled order, refund, allergy note, and overdue kitchen order. |

### 11.3 Date strategy

Seed dates relative to the current run date, not fixed calendar dates. A restaurant that runs today should have "today" orders, active clock-ins, and recent deliveries. Preserve deterministic randomization with a seed value (for example `20261001`) so the demo is repeatable while dates remain believable.

### 11.4 JSON shape (illustrative)

```json
{
  "seedVersion": 1,
  "stores": [{ "seedKey": "store-main", "name": "McDees Downtown", "timeZone": "America/Denver" }],
  "users": [{ "email": "jim@eathere.com", "roles": ["Customer"], "customerSeedKey": "customer-jim" }],
  "menuItems": [{ "seedKey": "classic-burger", "name": "Classic Burger", "basePrice": 5.49, "recipe": [{ "inventorySeedKey": "beef-patty", "quantity": 1 }] }],
  "orders": [{ "seedKey": "today-001", "storeSeedKey": "store-main", "relativeMinutes": -18, "status": "Preparing", "lines": [] }]
}
```

Do not place real passwords in JSON. For local demo identities, use a development-only known password documented in the README or generate it from a local secret; create users through `UserManager` so Identity fields/password hashes are valid.

## 12. Core Data Entities

Initial entities should include:

- **Identity and organization:** `ApplicationUser`, `CustomerProfile`, `Employee`, `EmployeeStoreAssignment`, `Store`, `Role`.
- **Menu:** `MenuCategory`, `MenuItem`, `MealDefinition`, `ModifierGroup`, `ModifierOption`, `MenuItemPrice`, `MenuItemImage`, `Allergen`, `Recipe`, `RecipeIngredient`.
- **Orders:** `Order`, `OrderLine`, `OrderLineModifier`, `OrderStatusHistory`, `Payment` (prototype-friendly status only at first).
- **Inventory:** `InventoryItem`, `StoreInventory`, `InventoryTransaction`, `Supplier`, `Delivery`, `DeliveryLine`.
- **Workforce:** `Position`, `Shift`, `TimeClockEntry`, `PayRateHistory`.

Use appropriate decimal precision for money/quantities and a database concurrency mechanism (for example PostgreSQL `xmin` or a concurrency token) on records likely to be edited concurrently, especially inventory and menu administration.

## 13. Suggested Implementation Milestones

### Milestone 1 — Foundation and seeded restaurant

Create ASP.NET Core MVC + Identity + PostgreSQL; roles; store-aware core entities; migrations; development-only idempotent seed service; basic layouts and authorization.

### Milestone 2 — Customer kiosk

Create visual menu/category browsing, item customization, cart, guest/signed-in order submission, confirmation/pickup code, and order persistence.

### Milestone 3 — Kitchen Production Display

Create live order queue, role-protected state transitions, order timers, status history, and simple polling refresh.

### Milestone 4 — Management essentials

Add menu/catalog administration, image upload, availability, price history, inventory viewing/adjustments, and basic manager dashboards.

### Milestone 5 — Workforce and receiving

Add employee records, scheduling, time clock, pay-rate history, delivery receiving/import, recipes, and inventory consumption.

### Milestone 6 — Loyalty and multi-store/corporate

Add customer loyalty, rewards, richer reporting, store overrides, additional locations, and corporate rollups.

## 14. Acceptance Criteria for the First Prototype

- A fresh developer setup creates PostgreSQL schema and a believable full-swing demo dataset with one documented command/startup action in Development.
- A guest can place a customized meal order from the customer kiosk and receive a pickup code.
- A signed-in customer can place an order and see it in order history.
- The new order appears on the Kitchen Production Display without manual database intervention.
- An authorized employee can advance it through kitchen states; an unauthorized customer cannot.
- A manager can change a menu item’s price/availability and upload/select an image without changing historical order details.
- The home/manager dashboard visibly demonstrates current orders, on-duty staff, inventory warnings, and recent activity.

## 15. Questions to Resolve During Prototyping

- Is the kiosk a dedicated browser/device mode, or merely a route within the shared web app for the prototype?
- Which tax rules apply, and are taxes included in displayed prices?
- Will orders be paid at the kiosk, paid at a counter, or simulated initially?
- Which KPD transitions can cooks perform, and which require manager approval?
- What is the official pickup/order numbering convention?
- When should inventory be consumed: preparation, completion, or sale/payment?
- Is corporate a single identity role, a separate tenant/organization model, or both in the eventual product?

These questions should not delay the kiosk/KPD prototype: use clearly named defaults and keep the resulting rules in services/configuration so they can be changed.

# Appendix A — Initial Brainstorm Query

i am creating a web app system using C# asp core mvc that will be used for a restaurant to manage inventory, orders, employees (schedules/shifts/check-in/check-out), have a "kiosk" for customers to order food on (for now all of these are from one web interface).  Will have users of roles - customer, employee, manager, owner, coorporate.  the kiosk can allow the customer to send in an order anonymously or log in and keep track of their orders over time (eventually for points and free food).  there should be the idea of individual stores (all which "report back" to cooporate) but for now the focus is on managing a single store (just keep functionality for the others). I want a typical hamburger establishment list of meals to start. I want 50 customers (e.g., a few named ones like: jim@eathere.com, and then a ubnch of customer1@eather.com).  I need a way to seed the entire application with data that looks like a restaurant that is in full swing (has been running for multiple months).    

You should also consider all other known types of functionalities on related apps, or what restaurant employees hae said about their needs).  

I need an overview document to present to an agent AI developer to begin prototyping this system.  Please format it as md, and use sections and sub-sections.  also order the document with a top level overview of all the main functionality, followed by sections for each use case (start with customer kiosk) and then employee kiosk (choose a better name, but the display that shows all the current orders).  There should also be employee features for checking in to work and leaving work, salaries, etc.  Again, it is very important to have a bunch of "seeded" data that represents the current status of the restaurant (supplies, food, menu, current customers food orders, current employees, which employees are at work, etc.) I suggest making a large json file with all of this info, but am open to other ideas.  

We will be using github to store the code under github user hjgermain in the repository Capstone/McDees/....  Again we are using C# with asp core and identity.

Again, the initial focus is the customer food ordering display menu and kiosk, and the employees "what to cook" display. There should be the ability to change prices, add images to food/menu items, customize ingrediants, etc.  Eventually we have the ability to accept food order shipmnets (some sort of standard "here is what we are delivering" list that is then automatically scanned and used to update the DB, (and of course, when a meal is cooked, the DB is updated to show the use of those foods/materials).  

Build this document for me.  Make sure the title page attributes this to me and gives the name of the application McDees and a short paragraph or two overview of the purpose of the project. there should be a section of major features with subsections for those I've mentioned and other ideas. Each section should have enough info to help an AI start coding.  There should be a software tools/process section early on that details the tools we are using (including postgres as the SQL server for our DB).  Give it a shot.

When the program starts up the

