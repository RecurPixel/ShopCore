# ShopCore - Complete API Routes Structure

**RESTful, role-based, clean hierarchy**

---

## 🔑 Authentication & Authorization

### **Auth** - `/api/v1/auth`
**Access:** Public

```http
POST   /register              # Register new user (Customer/Vendor)
POST   /login                 # Login, get JWT tokens
POST   /logout                # Logout (invalidate refresh token)
POST   /refresh-token         # Refresh access token
POST   /forgot-password       # Request password reset email
POST   /reset-password        # Reset password with token
POST   /verify-email          # Verify email with token
POST   /resend-verification   # Resend verification email
```

---

## 👤 User Self-Service

### **Users (Me)** - `/api/v1/users/me`
**Access:** Authenticated users only

```http
# Profile
GET    /                      # Get my profile
PUT    /                      # Update my profile (name, phone)
POST   /avatar                # Upload profile picture
POST   /change-password       # Change password
DELETE /                      # Delete my account (soft delete)

# Addresses
GET    /addresses             # List my addresses
POST   /addresses             # Add new address (with geolocation)
GET    /addresses/{id}        # Get single address
PUT    /addresses/{id}        # Update address
DELETE /addresses/{id}        # Delete address
PATCH  /addresses/{id}/default # Set as default

# My Orders (E-commerce)
GET    /orders                # List my orders (paginated, filtered)
GET    /orders/{id}           # Get order details
POST   /orders/{id}/cancel    # Cancel entire order (if allowed)
POST   /orders/items/{itemId}/cancel  # Cancel single item
GET    /orders/{id}/invoice   # Download order invoice (PDF)

# My Subscriptions (Private)
GET    /subscriptions         # List my subscriptions (Active/Paused/etc)
GET    /subscriptions/{id}    # Get subscription details
GET    /subscriptions/{id}/deliveries  # My deliveries for this subscription
GET    /subscriptions/{id}/invoices    # My invoices for this subscription

# My Reviews
GET    /reviews               # My reviews
POST   /reviews               # Create review (for purchased product)
PUT    /reviews/{id}          # Update my review
DELETE /reviews/{id}          # Delete my review

# My Wishlist
GET    /wishlist              # My wishlist items
POST   /wishlist              # Add product to wishlist
DELETE /wishlist/{productId}  # Remove from wishlist

# Payment History
GET    /payments              # My payment history
```

**Decision:** ✅ Yes, addresses under `/users/me/addresses` - addresses belong to users.

---

## 🏪 Vendor Management

### **Vendors** - `/api/v1/vendors`

#### **Public Endpoints** (No auth required)
```http
GET    /search                # Search vendors by location (pincode/lat-long)
GET    /{id}                  # Public vendor profile (storefront)
GET    /{id}/products         # Vendor's public products
```

#### **Vendor Self-Management** - `/me` (Vendor role required)
```http
# Profile
POST   /register              # Register as vendor (creates VendorProfile)
GET    /me                    # My vendor profile
PUT    /me                    # Update profile (business info, bank details)
POST   /me/logo               # Upload business logo
GET    /me/stats              # Statistics (products, orders, revenue)

# Service Areas (NEW)
GET    /me/service-areas      # List my service areas
POST   /me/service-areas      # Add service area (pincodes)
PUT    /me/service-areas/{id} # Update service area
DELETE /me/service-areas/{id} # Remove service area

# Customer Invitations (NEW)
GET    /me/invitations        # List invitations I sent
POST   /me/invitations        # Create customer invitation
GET    /me/invitations/{id}   # Get invitation details
POST   /me/invitations/{id}/resend  # Resend invitation
DELETE /me/invitations/{id}   # Cancel invitation

# Products
GET    /me/products           # My products
POST   /me/products           # Create product
GET    /me/products/{id}      # Get product details
PUT    /me/products/{id}      # Update product
DELETE /me/products/{id}      # Delete product (soft)
PATCH  /me/products/{id}/status  # Update status (Active/OutOfStock)
POST   /me/products/{id}/images # Upload product images
DELETE /me/products/{id}/images/{imageId}  # Delete image

# E-Commerce Orders (Orders with my products)
GET    /me/orders             # Orders containing my products
GET    /me/orders/{orderId}/items  # My items in this order
PATCH  /me/orders/items/{itemId}/status  # Update my order item status

# Subscriptions (Subscriptions for my products) - PRIVATE
GET    /me/subscriptions      # Subscriptions for my products
GET    /me/subscriptions/{id} # Subscription details
GET    /me/subscriptions/{id}/customer  # Customer info for this subscription

# Deliveries - PRIVATE
GET    /me/deliveries         # All my deliveries (calendar view)
GET    /me/deliveries?date=2025-01-27  # Deliveries for specific date
GET    /me/deliveries/{id}    # Delivery details
PATCH  /me/deliveries/{id}/complete  # Mark delivery complete
PATCH  /me/deliveries/{id}/failed    # Mark delivery failed
POST   /me/deliveries/{id}/photo     # Upload delivery proof

# Invoices - PRIVATE
GET    /me/invoices           # Invoices I generated
GET    /me/invoices/{id}      # Invoice details
POST   /me/subscriptions/{id}/invoices  # Manually generate invoice

# Payouts
GET    /me/payouts            # My payout history
GET    /me/payouts/pending    # Pending payout amount

# Customers (Vendor's view of their customers)
GET    /me/customers          # List customers with active subscriptions/orders
GET    /me/customers/{userId} # Customer details
GET    /me/customers/{userId}/subscriptions  # This customer's subscriptions with me
GET    /me/customers/{userId}/orders         # This customer's orders with me
GET    /me/customers/{userId}/deliveries     # This customer's delivery history
```

**Decision:** ✅ Yes, `/vendors/me/customers` needed - vendors need to see customer history for relationship management.

---

## 🛒 Public Catalog & Shopping

### **Categories** - `/api/v1/categories`
**Access:** Public + Admin

```http
# Public
GET    /                      # List all categories (tree structure)
GET    /{id}                  # Get category details
GET    /{id}/products         # Products in this category

# Admin Only
POST   /                      # Create category
PUT    /{id}                  # Update category
DELETE /{id}                  # Delete category
```

### **Products** - `/api/v1/products`
**Access:** Public (Read), Vendor (Write via /vendors/me/products)

```http
# Public
GET    /                      # List products (paginated, filtered, sorted)
GET    /search?q={query}      # Search products
GET    /featured              # Featured products
GET    /{id}                  # Get product details
GET    /{id}/reviews          # Product reviews

# Note: Product creation/management is under /vendors/me/products
```

### **Reviews** - `/api/v1/products/{productId}/reviews`
**Access:** Public (Read), Customer (Write)

```http
GET    /                      # List product reviews
POST   /                      # Create review (Customer only, purchased only)
POST   /{reviewId}/helpful    # Mark review helpful
POST   /{reviewId}/respond    # Vendor response to review
```

---

## 🛒 Shopping Cart

### **Cart** - `/api/v1/cart`
**Access:** Customer only

```http
GET    /                      # Get my cart
POST   /items                 # Add item to cart
PUT    /items/{itemId}        # Update item quantity
DELETE /items/{itemId}        # Remove item
DELETE /clear                 # Clear entire cart
POST   /validate              # Validate cart before checkout
POST   /apply-coupon          # Apply coupon code
DELETE /remove-coupon         # Remove coupon
```

---

## 📦 Orders (E-Commerce)

### **Orders** - `/api/v1/orders`
**Access:** Customer (create, view own), Vendor (view/update items), Admin (all)

```http
# Customer
POST   /                      # Create order (checkout from cart)
GET    /                      # List my orders (in /users/me/orders)
GET    /{id}                  # Get order details (in /users/me/orders/{id})
POST   /{id}/cancel           # Cancel order (in /users/me/orders/{id}/cancel)

# Note: Order creation is main endpoint, viewing is under /users/me/orders
# Vendor updates are under /vendors/me/orders
```

---

## 💳 Payments

### **Payments** - `/api/v1/payments`
**Access:** Authenticated

```http
POST   /orders/{orderId}/create-intent    # Create payment for order
POST   /invoices/{invoiceId}/create-intent # Create payment for invoice
POST   /confirm                            # Confirm payment
POST   /webhook                            # Payment gateway webhook (no auth)
GET    /                                   # Payment history (in /users/me/payments)
```

---

## 🔄 Subscriptions (PRIVATE)

### **Subscriptions** - `/api/v1/subscriptions`
**Access:** Customer (create, manage own), Vendor (view, manage deliveries)

```http
# Customer
POST   /                      # Create subscription (multi-item)
GET    /                      # List my subscriptions (in /users/me/subscriptions)
GET    /{id}                  # Get details (in /users/me/subscriptions/{id})
PATCH  /{id}                  # Update (quantity, frequency)
POST   /{id}/items            # Add one-time item to subscription
POST   /{id}/pause            # Pause subscription
POST   /{id}/resume           # Resume subscription
POST   /{id}/settle           # Settle and cancel subscription
DELETE /{id}                  # Cancel subscription

# Customer - Deliveries
GET    /{id}/deliveries       # List deliveries for this subscription
POST   /deliveries/{deliveryId}/skip  # Skip upcoming delivery

# Customer - Invoices
GET    /{id}/invoices         # List invoices for this subscription
POST   /invoices/{invoiceId}/pay      # Pay invoice

# One-Time Deliveries (for new customers trying out)
POST   /one-time-delivery     # Create one-time delivery
POST   /{id}/convert           # Convert one-time to regular subscription
```

### **Deliveries** - `/api/v1/deliveries`
**Access:** Customer (view own), Vendor (manage)

```http
# Customer endpoints are under /subscriptions/{id}/deliveries
# Vendor endpoints are under /vendors/me/deliveries

GET    /{id}                  # Get delivery details
GET    /{id}/download-receipt # Download delivery receipt (PDF)
```

### **Invoices** - `/api/v1/invoices`
**Access:** Customer (view, pay), Vendor (generate, view)

```http
# Customer endpoints are under /subscriptions/{id}/invoices
# Vendor endpoints are under /vendors/me/invoices

GET    /{id}                  # Get invoice details
GET    /{id}/download         # Download invoice (PDF)
```

---

## 🎟️ Coupons

### **Coupons** - `/api/v1/coupons`
**Access:** Public (list active), Admin (manage)

```http
# Public
GET    /active                # List active coupons
POST   /validate              # Validate coupon code

# Admin Only
GET    /                      # List all coupons
POST   /                      # Create coupon
PUT    /{id}                  # Update coupon
PATCH  /{id}/deactivate       # Deactivate coupon
DELETE /{id}                  # Delete coupon
```

---

## 🌍 Location & Search

### **Location** - `/api/v1/location`
**Access:** Public

```http
POST   /geocode               # Geocode address (get lat/long from address)
POST   /reverse-geocode       # Reverse geocode (get address from lat/long)
GET    /vendors/nearby        # Find vendors near location
GET    /pincodes/{pincode}/vendors  # Vendors serving this pincode
```

---

## 📨 Customer Invitations (Public)

### **Invitations** - `/api/v1/invitations`
**Access:** Public (accept), Vendor (manage via /vendors/me/invitations)

```http
# Public
GET    /{token}               # Get invitation details (public link)
POST   /{token}/accept        # Accept invitation (one-click signup)
POST   /{token}/reject        # Reject invitation

# Vendor management is under /vendors/me/invitations
```

---

## 👨‍💼 Admin

### **Admin** - `/api/v1/admin`
**Access:** Admin role only

```http
# Dashboard
GET    /dashboard             # Admin dashboard stats

# Users
GET    /users                 # List all users (paginated, filtered)
GET    /users/{id}            # Get any user details
PUT    /users/{id}            # Update any user
PATCH  /users/{id}/status     # Activate/suspend user
DELETE /users/{id}            # Delete user

# Vendors
GET    /vendors               # List all vendors
GET    /vendors/pending       # Vendors pending approval
PATCH  /vendors/{id}/approve  # Approve vendor
PATCH  /vendors/{id}/suspend  # Suspend vendor
PATCH  /vendors/{id}/activate # Reactivate vendor

# Products
GET    /products              # All products (for moderation)
PATCH  /products/{id}/feature # Feature/unfeature product

# Orders
GET    /orders                # All orders
GET    /orders/{id}           # Order details

# Subscriptions
GET    /subscriptions         # All subscriptions
GET    /subscriptions/{id}    # Subscription details

# Payouts
GET    /payouts               # All vendor payouts
POST   /payouts/calculate     # Calculate pending payouts
POST   /payouts               # Create payout
PATCH  /payouts/{id}/process  # Process payout
PATCH  /payouts/{id}/cancel   # Cancel payout

# Reports
GET    /reports/revenue       # Revenue report
GET    /reports/vendors       # Vendor performance
GET    /reports/products      # Product analytics
GET    /reports/customers     # Customer analytics
```

---

## 📊 Summary by Controller

### Controllers Needed:

1. ✅ **AuthController** - Authentication only
2. ✅ **UsersController** - Self-service (`/users/me/*`)
3. ✅ **VendorsController** - Vendor management (`/vendors/*`)
4. ✅ **CategoriesController** - Product categories
5. ✅ **ProductsController** - Public product catalog
6. ✅ **CartController** - Shopping cart
7. ✅ **OrdersController** - Order creation
8. ✅ **PaymentsController** - Payment processing
9. ✅ **SubscriptionsController** - Subscription management (PRIVATE)
10. ✅ **DeliveriesController** - Delivery details (PRIVATE)
11. ✅ **InvoicesController** - Invoice details (PRIVATE)
12. ✅ **CouponsController** - Coupon management
13. ✅ **LocationController** - Location services
14. ✅ **InvitationsController** - Customer invitations (public accept)
15. ✅ **AdminController** - Admin operations

---

## 🔐 Authorization Matrix

| Endpoint Pattern | Customer | Vendor | Admin |
|-----------------|----------|--------|-------|
| `/auth/*` | ✅ | ✅ | ✅ |
| `/users/me/*` | ✅ (own) | ✅ (own) | ✅ (own) |
| `/vendors/search` | ✅ | ✅ | ✅ |
| `/vendors/{id}` (public) | ✅ | ✅ | ✅ |
| `/vendors/me/*` | ❌ | ✅ (own) | ❌ |
| `/products` (read) | ✅ | ✅ | ✅ |
| `/cart/*` | ✅ | ❌ | ❌ |
| `/orders` (create) | ✅ | ❌ | ❌ |
| `/subscriptions` (create) | ✅ | ❌ | ❌ |
| `/subscriptions` (manage deliveries) | ❌ | ✅ | ✅ |
| `/admin/*` | ❌ | ❌ | ✅ |

---

## 🎯 Key Design Decisions

### ✅ **Decision 1: Addresses under `/users/me/addresses`**
**Rationale:** Addresses belong to users, not orders or subscriptions. Both can reference the same address.

### ✅ **Decision 2: Include `/vendors/me/customers`**
**Rationale:** Vendors need relationship management - see customer history, subscriptions, delivery patterns.

### ✅ **Decision 3: Separate `/admin/*` namespace**
**Rationale:** Clear separation of concerns, easier to secure, better for audit logs.

### ✅ **Decision 4: Nested vs Flat Resources**

**Nested when:**
- Tight coupling (e.g., `/subscriptions/{id}/invoices`)
- Parent-child relationship
- No independent access needed

**Flat when:**
- Resource accessed from multiple contexts
- Independent operations
- Simpler queries

**Examples:**
- ✅ Nested: `/subscriptions/{id}/deliveries` (deliveries belong to subscription)
- ✅ Flat: `/products` (products browsed independently)
- ✅ Both: Deliveries accessible via `/subscriptions/{id}/deliveries` AND `/vendors/me/deliveries`

### ✅ **Decision 5: Duplicate Endpoints for Different Contexts**

**Example:** Orders appear in 3 places:
1. `/users/me/orders` - Customer views their orders
2. `/vendors/me/orders` - Vendor views orders with their products
3. `/admin/orders` - Admin views all orders

**Rationale:** Each has different filtering, permissions, and data shaping needs.

---

## 📝 Implementation Notes

### **Controller Routing Attributes:**

```csharp
// AuthController
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase { }

// UsersController (Me endpoints)
[ApiController]
[Route("api/v1/users/me")]
[Authorize] // All require auth
public class UsersController : ControllerBase { }

// VendorsController
[ApiController]
[Route("api/v1/vendors")]
public class VendorsController : ControllerBase 
{
    // Public endpoints - no [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> Search() { }
    
    // Vendor-only endpoints - [Authorize(Roles="Vendor")]
    [HttpGet("me")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> GetMyProfile() { }
}

// AdminController
[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")] // All require Admin
public class AdminController : ControllerBase { }
```

### **Query Parameter Standards:**

```http
# Pagination
?page=1&pageSize=20

# Filtering
?status=Active&categoryId=5

# Sorting
?sortBy=createdAt&sortOrder=desc

# Search
?q=search+term

# Date range
?from=2025-01-01&to=2025-01-31

# Location
?pincode=560034
?lat=12.9352&lng=77.6245&radius=5
```

---

## 🚀 Migration from Current Structure

### **Before (Scattered):**
- `/users/me` - Profile
- `/addresses` - Addresses (separate)
- `/vendors` - Vendor stuff
- `/orders` - Orders

### **After (Clean):**
- `/users/me/*` - Everything about me
- `/vendors/me/*` - Everything about my vendor business
- `/admin/*` - Admin-only operations

### **Migration Steps:**
1. Keep old routes working (mark deprecated)
2. Add new routes
3. Update frontend to use new routes
4. Remove old routes after frontend migration

---

**This structure is:**
- ✅ RESTful and intuitive
- ✅ Role-based with clear boundaries
- ✅ Scalable (easy to add features)
- ✅ Consistent (predictable patterns)
- ✅ Self-documenting (URL tells you what it does)
