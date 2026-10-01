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

## Appendix: AI development conversation and effort record

This record captures the development conversation that produced the current prototype. It is intended to help set practical expectations: AI can move quickly through a well-scoped prototype, but the elapsed calendar time also includes human decisions, authentication/approval steps, local-environment setup, validation, and review time.

### Configuration

- **Agent:** Codex, GPT-5-based coding agent.
- **Reasoning level:** The desktop task metadata does not expose a named reasoning-effort setting for this session; no level is inferred here.
- **Time zone:** America/Denver (`-06:00` on 2026-10-01).
- **Measurement snapshot:** 2026-10-01 15:37:44 -06:00, immediately after the request for this appendix.

### Effort summary

- **Conversation span:** 2 hours 24 minutes 32 seconds, from the first request at 13:13:12 through the appendix request at 15:37:44.
- **Recorded active AI work:** 26 minutes 28 seconds across completed task turns. This is a tool-recorded total, not an estimate of equivalent human development hours.
- **User requests recorded:** 12, including repository setup, source-document import, prototype construction, database/authentication, operations views, live updates, and this documentation request.
- **Delivery cadence:** 6 implementation commits were created and pushed before this appendix was requested.

### User request timeline

| Timestamp | Request / decision |
| --- | --- |
| 2026-10-01 13:13:12 -06:00 | Asked to connect the workspace to GitHub for pull, commit, and push operations. |
| 2026-10-01 13:13:43 -06:00 | Asked the agent to perform setup; supplied GitHub user `hjgermain`. |
| 2026-10-01 13:15:50 -06:00 | Supplied the `hjgermain/Capstone` repository and commit email. |
| 2026-10-01 13:17:34 -06:00 | Requested import of the McDees prototype overview into the repository. |
| 2026-10-01 13:18:29 -06:00 | Requested the overview document be committed. |
| 2026-10-01 13:18:52 -06:00 | Set the default practice of pushing after commits unless there is a reason not to. |
| 2026-10-01 13:20:59 -06:00 | Requested the core system through a customer self-order kiosk prototype. |
| 2026-10-01 13:30:50 -06:00 | Requested local user-secrets configuration for PostgreSQL. |
| 2026-10-01 13:35:48 -06:00 | Requested Identity users, role-aware landing pages, and an admin database rebuild control without migrations. |
| 2026-10-01 14:16:57 -06:00 | Requested the employee kitchen queue and manager inventory overview. |
| 2026-10-01 14:26:22 -06:00 | Requested live kiosk order-status display and removal of the acknowledged state. |
| 2026-10-01 15:37:44 -06:00 | Requested this AI conversation and effort appendix. |

### Commit timeline before this appendix

| Timestamp | Commit | Delivery |
| --- | --- | --- |
| 2026-10-01 13:18:39 -06:00 | `1be6c4a` | Added the prototype overview. |
| 2026-10-01 13:27:37 -06:00 | `b93371d` | Built the customer kiosk prototype. |
| 2026-10-01 13:31:26 -06:00 | `4a31bfb` | Configured local user-secrets support. |
| 2026-10-01 13:40:56 -06:00 | `116cbf4` | Added Identity roles and prototype database administration. |
| 2026-10-01 14:21:52 -06:00 | `1ff3219` | Added the kitchen queue and inventory overview. |
| 2026-10-01 14:29:55 -06:00 | `6bdac7c` | Added live kiosk order status updates. |

### Interpretation

In this session, the AI accelerated a prototype from an empty repository to a running MVC application with a kiosk, Identity roles, PostgreSQL-backed prototype data, a manager inventory view, a kitchen queue, and real-time kiosk status updates. This outcome should be treated as a prototype velocity benchmark, not a production estimate: production readiness still requires requirements refinement, tests, durable order/inventory workflows, security review, deployment, monitoring, accessibility review, and a migration strategy.

## Prototype limits

Orders and the menu are currently in memory. Restarting the application clears them. No payment, authentication, inventory, or kitchen queue persistence has been added yet.
