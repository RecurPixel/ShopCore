# ShopCore

A production-ready multi-vendor backend that combines traditional e-commerce with a subscription-based recurring delivery system. Built for local vendors (dairy, water, groceries) who deliver daily and bill monthly.

Built with ASP.NET Core 10, Clean Architecture, and CQRS.

---

## What Makes It Different

Most e-commerce backends handle one-off orders. ShopCore handles both:

**E-Commerce** — customers browse products, add to cart, checkout, pay per order.

**Subscriptions** — customers subscribe to recurring deliveries (e.g., 1L milk daily). Deliveries happen every day; billing happens monthly. Vendors track delivery status per item. Invoices auto-generate at the end of each billing cycle. Customers pay one invoice instead of 30 individual payments.

This combination is built for markets like India where local vendors (doodhwalas, newspaper distributors, water can suppliers) already do daily home delivery but manage everything manually.

---

## Architecture

```
src/
  ShopCore.Domain/          # Entities, enums, value objects — no external dependencies
  ShopCore.Application/     # CQRS commands/queries (MediatR), FluentValidation, interfaces
  ShopCore.Infrastructure/  # EF Core, payment gateways, notification service, file storage
  ShopCore.API/             # ASP.NET Core controllers, middleware, DI configuration

tests/
  ShopCore.UnitTests/       # Integration tests via WebApplicationFactory + InMemory DB
```

Clean Architecture — all dependencies point inward. Domain knows nothing about EF Core, HTTP, or any external library.

---

## Tech Stack

| Concern       | Choice                                               |
| ------------- | ---------------------------------------------------- |
| Framework     | ASP.NET Core 10                                      |
| ORM           | Entity Framework Core 10 (SQL Server)                |
| CQRS          | MediatR                                              |
| Validation    | FluentValidation                                     |
| Auth          | BCrypt + JWT (refresh tokens)                        |
| Payments      | Razorpay, Stripe, Cash on Delivery                   |
| Notifications | RecurPixel.Notify.Sdk (email, in-app; multi-channel) |
| PDF           | QuestPDF                                             |
| File Storage  | Local / Azure Blob                                   |
| Mapping       | Manual                                               |
| Testing       | xUnit + WebApplicationFactory                        |

---

## Key Features

**Auth**
- Register, login, JWT + refresh tokens, email verification, password reset

**Multi-Vendor**
- Vendors register, set up product catalogs, define service areas (by pincode)
- Admin approves/suspends vendors
- Per-item order status — each vendor updates their own items independently
- Order status is derived from item statuses (no manual override)

**E-Commerce**
- Cart, checkout, multi-vendor order splitting, COD + online payment
- Order cancellation, partial cancellation, refunds
- Reviews, wishlist, coupons, PDF invoices

**Subscriptions**
- Multi-item subscriptions (milk + eggs + newspaper in one subscription)
- Daily delivery, monthly billing — one invoice per cycle
- Optional deposit system with settlement on cancellation
- Skip deliveries, pause/resume subscriptions
- One-time item additions to existing subscriptions
- Vendor-led customer onboarding via invitation links

**Payments**
- Razorpay and Stripe integration with webhook-verified order confirmation
- Partial and full refunds
- Cash on Delivery support

**Notifications**
- Multi-channel delivery via RecurPixel.Notify — email and in-app
- In-app notifications persisted to DB via SDK hook
- Delivery audit log (every send attempt logged with status)
- Triggered for: registration, order placed/cancelled, refund, invoice paid, vendor approval, payouts

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (Express or LocalDB is fine for development)

### 1. Clone

```bash
git clone https://github.com/RecurPixel/ShopCore.git
cd ShopCore
```

### 2. Configure

Copy the example settings file and fill in your values:

```bash
cp src/ShopCore.API/appsettings.example.json src/ShopCore.API/appsettings.json
```

| Setting                               | Description                                       |
| ------------------------------------- | ------------------------------------------------- |
| `ConnectionStrings.DefaultConnection` | SQL Server connection string                      |
| `JwtSettings.Secret`                  | Random string, minimum 32 characters              |
| `Notify.Email.Smtp.*`                 | SMTP credentials (Mailtrap works for development) |
| `PaymentGateways.Razorpay.*`          | Razorpay Key ID + Secret (use test keys)          |
| `PaymentGateways.Stripe.*`            | Stripe publishable + secret keys (use test keys)  |

### 3. Database

```bash
dotnet ef database update \
  --project src/ShopCore.Infrastructure \
  --startup-project src/ShopCore.API
```

### 4. Run

```bash
dotnet run --project src/ShopCore.API
```

Swagger UI available at `https://localhost:5001/swagger`.

### 5. Tests

```bash
dotnet test
```

Tests use an in-memory database — no SQL Server required.

---

## Payment Webhooks

Both gateways confirm payment via webhook before marking an order as paid.

| Gateway  | Webhook Endpoint                         |
| -------- | ---------------------------------------- |
| Razorpay | `POST /api/v1/payments/webhook/razorpay` |
| Stripe   | `POST /api/v1/payments/webhook/stripe`   |

Configure these URLs in your gateway dashboard. The webhook secret goes in `appsettings.json` under `PaymentGateways.Razorpay.WebhookSecret` / `PaymentGateways.Stripe.WebhookSecret`.

---

## API Overview

Full reference available via Swagger. Key route groups:

| Prefix                  | Roles           | Description                                              |
| ----------------------- | --------------- | -------------------------------------------------------- |
| `/api/v1/auth`          | Public          | Register, login, verify email, password reset            |
| `/api/v1/products`      | Public          | Catalog browse, search                                   |
| `/api/v1/categories`    | Public          | Category listing                                         |
| `/api/v1/users/me`      | Customer        | Profile, addresses, orders, subscriptions, notifications |
| `/api/v1/cart`          | Customer        | Cart management                                          |
| `/api/v1/orders`        | Customer        | Place and manage orders                                  |
| `/api/v1/subscriptions` | Customer        | Subscription lifecycle                                   |
| `/api/v1/deliveries`    | Customer/Vendor | Delivery tracking and completion                         |
| `/api/v1/payments`      | Customer        | Payment initiation and webhooks                          |
| `/api/v1/vendors/me`    | Vendor          | Dashboard, products, orders, invitations, payouts        |
| `/api/v1/admin`         | Admin           | Vendor approval, coupons, payouts, platform stats        |

---

## License

MIT
