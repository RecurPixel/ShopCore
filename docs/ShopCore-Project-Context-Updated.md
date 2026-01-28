# ShopCore - Updated Project Context Document

**Version:** 2.0  
**Last Updated:** January 26, 2026  
**Status:** Active Development

---

## 🎯 Executive Summary

**ShopCore** is a dual-purpose platform combining traditional e-commerce with an innovative subscription-based recurring delivery system for daily essentials, targeting the Indian market.

### **Strategic Approach**

**One Backend, Two Frontends:**
- **Public E-Commerce** (GitHub portfolio) - Traditional multi-vendor marketplace
- **Private Subscription** (Market product) - Subscription platform with competitive moat

**Competitive Advantages:**
1. ✅ Flexible billing cycles (daily delivery, monthly payment = 90% lower fees)
2. ✅ Optional deposit system with smart settlement
3. ✅ Multi-item subscriptions (milk + newspaper + bread in one subscription)
4. ✅ Vendor-led customer onboarding (viral growth)
5. ✅ Location-based vendor discovery

---

## 🏗️ System Architecture

### **Technology Stack**

**Backend:**
- .NET 8 Web API
- Clean Architecture + CQRS (MediatR)
- Entity Framework Core 8 (Code-First)
- SQL Server (LocalDB dev, Azure SQL prod)
- JWT Bearer Authentication
- FluentValidation (input validation)
- AutoMapper (object mapping)
- Hangfire (background jobs)

**Frontend (Two Separate Apps):**
- React/Next.js (both apps)
- TailwindCSS (styling)
- Axios (API calls)
- Redux/Zustand (state management)

**External Services:**
- Payment: Razorpay (India) / Stripe (International)
- Email: SendGrid / SMTP
- SMS: Twilio / MSG91
- WhatsApp: Twilio WhatsApp API
- Maps: Google Maps API (location, geocoding)
- Storage: Local filesystem (dev) / Azure Blob (prod)

### **Architecture Pattern**

```
┌─────────────────────────────────────────────────────┐
│           ShopCore.API (Single Backend)              │
│                                                      │
│  ┌──────────────────┐  ┌──────────────────────┐   │
│  │  Public Features │  │  Private Features     │   │
│  │  ----------------│  │  -------------------  │   │
│  │  • Auth          │  │  • Subscriptions      │   │
│  │  • Products      │  │  • Deliveries         │   │
│  │  • Cart          │  │  • Invoices           │   │
│  │  • Orders        │  │  • Settlement         │   │
│  │  • Payments      │  │  • Deposit System     │   │
│  │  • Reviews       │  │  • One-Time Additions │   │
│  └──────────────────┘  └──────────────────────┘   │
└─────────────────────────────────────────────────────┘
              │                        │
              │                        │
    ┌─────────┴────────┐    ┌─────────┴──────────┐
    │   Frontend #1    │    │    Frontend #2     │
    │   (Public)       │    │    (Private)       │
    ├──────────────────┤    ├────────────────────┤
    │ E-Commerce UI    │    │ Subscription UI    │
    │ • Browse         │    │ • Subscribe        │
    │ • Cart           │    │ • Manage Items     │
    │ • Checkout       │    │ • View Deliveries  │
    │ • Track Orders   │    │ • Pay Invoices     │
    └──────────────────┘    └────────────────────┘
         │                           │
         │                           │
    GitHub Public              Private Repo
    (Portfolio)               (Real Business)
```

---

## 🎯 Target Market & Users

### **Demand Side (Customers)**

**B2C Households (70% revenue target):**
- Urban families in Tier 1/2 cities
- Working professionals (no time for daily shopping)
- Pain: Forgetting to order, running out unexpectedly
- Need: Milk, water, bread, eggs DAILY without thinking
- Behavior: Set and forget, pay monthly

**B2B Small Offices (20% revenue):**
- Startups, small companies (10-50 employees)
- Need: Water cans, tea/coffee, snacks
- Want: Predictable supply, single vendor
- Behavior: Subscribe, monthly billing

**One-time Buyers (10% revenue):**
- Tourists, temporary residents
- Trying products before subscribing
- Impulse purchases

### **Supply Side (Vendors)**

**Traditional Dairy Vendors (Primary - Perfect Fit):**
- Already do daily home delivery
- Track customers manually (notebook)
- Pain: Cash collection chaos, no digital records
- Our value: Digital payment tracking + monthly billing

**Water Suppliers (Primary - Perfect Fit):**
- Already subscription-based (20L cans)
- Already take deposits (₹100-200 per can)
- Our value: Automated billing, deposit management

**Local Kirana Stores (Secondary):**
- Handle walk-in + one-time orders
- Not naturally set up for daily delivery
- May adopt for add-on items

**Newspaper Distributors (Secondary):**
- Already subscription-based
- Daily delivery model
- Simple product catalog

---

## 📊 Database Schema (Updated)

### **Total Tables: 23**

**Core (7 tables):**
1. User
2. VendorProfile
3. Address
4. Category
5. Product
6. ProductImage
7. ProductSpecification

**E-Commerce (9 tables):**
8. Cart
9. CartItem
10. Order
11. OrderItem
12. OrderStatusHistory
13. Review
14. Wishlist
15. Coupon
16. VendorPayout

**Subscriptions - Private (5 tables):**
17. Subscription
18. SubscriptionItem
19. Delivery
20. DeliveryItem
21. SubscriptionInvoice

**Location & Onboarding (2 tables):**
22. VendorServiceArea
23. CustomerInvitation

### **Key Relationships**

```
User (1) ──────┬──── (1) VendorProfile ──── (N) VendorServiceArea
               │                        └──── (N) CustomerInvitation
               ├──── (N) Address
               ├──── (1) Cart ──── (N) CartItem
               ├──── (N) Order ──── (N) OrderItem
               │                └──── (N) OrderStatusHistory
               ├──── (N) Review
               ├──── (N) Wishlist
               └──── (N) Subscription ──── (N) SubscriptionItem
                                      ├──── (N) Delivery ──── (N) DeliveryItem
                                      └──── (N) SubscriptionInvoice

Category (1) ──── (N) Product ──── (N) ProductImage
                                │
                                ├──── (N) ProductSpecification
                                ├──── (N) CartItem
                                ├──── (N) OrderItem
                                ├──── (N) Review
                                ├──── (N) Wishlist
                                └──── (N) SubscriptionItem
```

---

## 🔄 User Flows & Onboarding

### **Primary Flow (90%): Vendor-Led Onboarding**

```
Existing Offline Relationship:
Ramu Doodhwala serves 50 families (notebook tracking)

ShopCore Sales Team Onboards Ramu:
├─ Creates vendor account
├─ Vendor adds products
├─ Vendor defines service areas (pincodes)
└─ Vendor adds existing customers via invitation system

For Each Customer (e.g., Mr. Sharma):
├─ Vendor fills: Name, Phone, Address, Products, Delivery time
├─ System generates unique invitation link
├─ Sends via SMS/WhatsApp/Email
└─ Customer clicks link:
    ├─ Sees pre-filled subscription details
    ├─ One-click accept
    ├─ Account created automatically
    └─ Subscription activated

Result:
├─ Vendor migrates 50 customers in ~30 minutes
├─ 60-70% conversion rate (30-35 active customers)
└─ Zero friction for customers
```

**Why This Works:**
- ✅ Leverages existing trust relationship
- ✅ Pre-filled data (no forms for customer)
- ✅ Vendor controls onboarding
- ✅ Viral growth (vendors become distribution channel)
- ✅ Lower customer acquisition cost

### **Secondary Flow (10%): Customer Discovery**

```
New Customer (Priya):
├─ Downloads ShopCore app
├─ Signs up (phone/email)
├─ Adds delivery address (with location capture)
├─ System shows vendors serving her area (based on pincode/geofence)
├─ Browses products from nearby vendors
├─ Creates subscription or places one-time order
└─ Gets first delivery next day
```

---

## 🛒 E-Commerce System (Public)

### **Order Management**

**Key Architectural Decision: Order Status is DERIVED**

```csharp
Order {
    Status: COMPUTED from OrderItems (not directly set!)
    PaymentStatus: Tracked separately
    RefundedAmount: Separate tracking
}

OrderItem {
    Status: SET by vendor (Pending → Confirmed → Processing → Shipped → Delivered)
    VendorId: Each item belongs to one vendor
    CommissionRate: Snapshot at order time
}

// Order status logic
Order.UpdateStatusFromItems() {
    if (all items delivered) → Order.Status = Delivered
    if (some items delivered) → Order.Status = PartiallyDelivered
    if (all items cancelled) → Order.Status = Cancelled
    if (some cancelled) → Order.Status = PartiallyCancelled
    // ... etc
}
```

**Multi-Vendor Order Flow:**

```
Customer Cart:
├─ Item A from Vendor 1: ₹200
├─ Item B from Vendor 1: ₹150
└─ Item C from Vendor 2: ₹150

Checkout → Single Order Created:
├─ Order ID: #123
├─ Total: ₹500
├─ Payment: ₹500 (single payment)
└─ OrderItems:
    ├─ Item A (Vendor 1) - Status: Pending
    ├─ Item B (Vendor 1) - Status: Pending
    └─ Item C (Vendor 2) - Status: Pending

Each Vendor Manages Their Items Independently:
├─ Vendor 1 updates Items A & B: Confirmed → Processing → Shipped → Delivered
├─ Vendor 2 updates Item C: Confirmed → Processing → Shipped → Delivered
└─ Order.Status auto-updates based on all items

If Customer Cancels Item C Before Shipping:
├─ Item C status: Cancelled
├─ Order status: PartiallyCancelled (Items A, B still active)
├─ Refund processed: ₹150
└─ Order.PaymentStatus: PartiallyRefunded
```

**Status Enums:**

```csharp
public enum OrderStatus {
    Pending,              // Just created
    PaymentFailed,        // Payment failed
    Confirmed,            // Payment successful
    Processing,           // At least one vendor processing
    PartiallyShipped,     // Some items shipped
    Shipped,              // All items shipped
    PartiallyDelivered,   // Some items delivered
    Delivered,            // All items delivered
    Completed,            // Delivered + review period over
    Cancelled,            // All cancelled
    PartiallyCancelled,   // Some cancelled
    Refunded,             // Full refund
    PartiallyRefunded     // Partial refund
}

public enum OrderItemStatus {
    Pending,              // Awaiting vendor action
    Confirmed,            // Vendor accepted
    Processing,           // Vendor preparing
    Shipped,              // In transit
    Delivered,            // Delivered to customer
    Cancelled,            // Cancelled
    Refunded              // Refunded
}

public enum PaymentStatus {
    Unpaid,
    Pending,
    Paid,
    PartiallyRefunded,
    Refunded,
    Failed
}
```

---

## 📅 Subscription System (Private)

### **Core Concept: Subscription as Ongoing Relationship**

```
Subscription = Ongoing relationship with ONE vendor
├─ Regular Items (recurring: true)
│   ├─ Milk 1L daily
│   └─ Newspaper daily
│
└─ One-Time Items (recurring: false)
    ├─ Paneer 200g (deliver: Jan 25) - piggybacks on regular delivery
    └─ Eggs 6pc (deliver: Jan 27) - piggybacks on regular delivery
```

**Multi-Item Support:**

```
Customer: "Daily Dairy Bundle" Subscription
├─ VendorId: 10 (Fresh Dairy)
├─ Frequency: Daily
├─ BillingCycleDays: 30
├─ Items:
│   ├─ SubscriptionItem 1: Milk 1L × 1 = ₹60/day (recurring)
│   ├─ SubscriptionItem 2: Yogurt 500g × 1 = ₹40/day (recurring)
│   └─ SubscriptionItem 3: Paneer 200g × 2 = ₹180 (one-time, Jan 25)
│
├─ Daily Delivery Auto-Generated:
│   └─ Delivery (Jan 25):
│       ├─ DeliveryItem 1: Milk 1L - Delivered ✓
│       ├─ DeliveryItem 2: Yogurt 500g - Delivered ✓
│       └─ DeliveryItem 3: Paneer 200g × 2 - Out of Stock ✗
│
├─ Billing:
│   ├─ Total charged: ₹100 (only delivered items)
│   └─ UnpaidAmount += ₹100
│
└─ After 30 days:
    ├─ Invoice generated: ₹3000 (30 days × ₹100 average)
    └─ Customer pays monthly
```

**Deposit System:**

```
Subscription Creation:
├─ Customer: ₹500 deposit (optional, for water jars etc)
├─ Vendor: Sets requirement via VendorProfile.RequiresDeposit
└─ Deposit held throughout subscription

Monthly Flow:
├─ Deliveries happen regardless of payment
├─ Unpaid amount accumulates
├─ Invoice generated every billing cycle (e.g., 30 days)
├─ Customer pays invoice
└─ Deposit never touched during normal operations

If Unpaid Amount > Credit Limit (e.g., ₹1200):
├─ Status: Active → Suspended
├─ Deliveries stop
└─ Resume after payment

Subscription Cancellation (Settlement):
├─ Calculate: total_delivered - total_paid - deposit
├─ If positive: Customer owes vendor → Payment screen
├─ If negative: Vendor refunds customer → Refund initiated
└─ If zero: All settled → Subscription closed
```

**Flexible Billing Cycles:**

```
Option 1: Vendor Sets Explicit Cycle
├─ VendorProfile.DefaultBillingCycleDays = 30
└─ Invoice every 30 days

Option 2: Auto-Calculate from Deposit
├─ Deposit: ₹600
├─ Daily price: ₹20
├─ Billing cycle: ₹600 / ₹20 = 30 days (rounded down)
└─ Invoice generated on day 30
```

**Delivery Status (Separate from Orders):**

```csharp
public enum DeliveryStatus {
    Scheduled,         // Auto-generated, not yet delivered
    OutForDelivery,    // Driver picked up
    Delivered,         // Successfully delivered
    Failed,            // Delivery attempt failed
    Skipped,           // Customer skipped
    Cancelled          // Cancelled
}

public enum DeliveryItemStatus {
    Scheduled,         // Will be delivered
    Delivered,         // Successfully delivered
    OutOfStock,        // Vendor didn't have it
    Damaged,           // Product was damaged
    Skipped,           // Not delivered per customer request
    Cancelled          // Cancelled
}

// Payment status reused from orders
public enum PaymentStatus {
    Unpaid,
    Pending,
    Paid,
    PartiallyRefunded,
    Refunded,
    Failed
}
```

---

## 🌍 Location-Based Features

### **Vendor Service Areas**

```csharp
VendorServiceArea {
    VendorId: 5
    AreaName: "Koramangala"
    City: "Bangalore"
    State: "Karnataka"
    Pincodes: ["560034", "560035", "560095"]
    GeoJsonPolygon: null  // Optional geofencing for advanced
    IsActive: true
}
```

**Vendor can serve multiple areas:**
```
Ramu Dairy serves:
├─ Koramangala (560034, 560035, 560095)
├─ HSR Layout (560102, 560103)
└─ BTM Layout (560068, 560076)
```

**Customer Search:**
```
Customer in pincode 560034:
├─ System queries: VendorServiceAreas where Pincodes contains "560034"
├─ Returns: All vendors serving that pincode
└─ Shows sorted by: Rating, Distance, Product availability
```

### **Address with Geolocation**

```csharp
Address {
    AddressLine1: "123, MG Road"
    City: "Bangalore"
    State: "Karnataka"
    Pincode: "560034"
    Latitude: 12.9352    // Captured from map
    Longitude: 77.6245   // Captured from map
    PlaceId: "ChIJ..."   // Google Maps Place ID
    Landmark: "Near ICICI Bank"
    Type: Home           // Home, Office, Other
}
```

**Future Enhancement (Phase 2):**
- Distance calculation between vendor and customer
- Route optimization for deliveries
- Real-time tracking

---

## 🎯 Feature List (Complete)

### **Phase 1: Public E-Commerce (Hours 0-16)**

**1. Authentication & Authorization ✅**
- User registration (email, password, phone, role)
- User login (JWT token generation)
- Logout (invalidate refresh token)
- Password reset (email-based)
- Email verification
- Role-based authorization: Customer, Vendor, Admin

**2. User Management ✅**
- Get current user profile
- Update profile (name, phone)
- Upload profile picture
- Change password

**3. Address Management ✅**
- List user addresses
- Add new address (with geolocation capture)
- Update address
- Delete address
- Set default address

**4. Vendor Management ✅**
- Register as vendor
- Vendor profile (get/update)
- Define service areas (pincodes)
- Vendor statistics (products, orders, revenue)
- **Admin:** Approve/suspend vendors

**5. Category Management ✅**
- List categories (hierarchical tree)
- Get category details
- Get products in category
- **Admin:** Create/update/delete categories

**6. Product Management ✅**
- List products (pagination, filters, sorting)
- Get product details
- Search products (full-text)
- Get featured products
- **Vendor:** Create product
- **Vendor:** Update product
- **Vendor:** Delete product
- **Vendor:** Upload product images (multiple)
- **Vendor:** Update product status

**7. Shopping Cart ✅**
- Get cart (with totals)
- Add to cart (validate stock)
- Update cart item quantity
- Remove from cart
- Clear cart
- Validate cart (before checkout)

**8. Order Management ✅**
- Create order (checkout from cart)
- Multi-vendor order splitting
- List user orders (filter, pagination)
- Get order details
- Cancel order (if Pending/Confirmed)
- Cancel order item (if not shipped)
- Download invoice (PDF)
- **Vendor:** Update order item status

**9. Payment Integration ✅**
- Create payment intent (Razorpay/Stripe)
- Confirm payment
- Payment webhook
- Get payment history
- Process refunds (partial/full)
- Support: Online payment + Cash on Delivery (COD)

**10. Review & Rating System ✅**
- Get product reviews (pagination, sorting)
- Create review (1-5 stars, only for purchased products)
- Update review
- Delete review
- Mark review helpful
- **Vendor:** Respond to review

**11. Wishlist ✅**
- Get wishlist
- Add to wishlist
- Remove from wishlist

**12. Discount Coupons ✅**
- List active coupons
- Validate coupon code
- Apply coupon to order
- **Admin:** Create coupon
- **Admin:** Deactivate coupon

**13. Vendor Payouts ✅**
- Calculate vendor earnings
- Track commission deductions
- Generate payout reports
- **Admin:** Process payouts

### **Phase 2: Private Subscription (Hours 16-24)**

**14. Customer Invitation System (NEW) ✅**
- **Vendor:** Create customer invitation
- **Vendor:** Bulk invite customers (CSV import - future)
- **Vendor:** View invitation status
- **Vendor:** Resend invitation
- **Customer:** Accept invitation (one-click)
- **Customer:** Reject invitation
- Send via SMS/WhatsApp/Email

**15. Subscription Management ✅**
- Create subscription (multi-item)
- List user subscriptions (filter by status)
- Get subscription details
- Update subscription (add/remove items, change quantity)
- Add one-time item to existing subscription
- Create one-time delivery (for new customers)
- Pause subscription
- Resume subscription
- Cancel subscription (with settlement)
- Settle subscription (deposit refund/payment)

**16. Delivery Management ✅**
- List upcoming deliveries (calendar view)
- Get delivery details
- Mark delivery complete (by vendor/driver)
- Update delivery item status (per-item)
- Skip delivery (by customer)
- Reschedule failed delivery
- Payment on delivery (COD/UPI)
- Upload delivery proof (photo)

**17. Invoice Management ✅**
- Auto-generate invoices (cron job based on billing cycle)
- Manual invoice generation (vendor request)
- Get unpaid invoices
- Pay invoice (online payment)
- Download invoice (PDF)
- Invoice status tracking

**18. Location Services ✅**
- Search vendors by location (pincode/lat-long)
- Geocode addresses (Google Maps API)
- Calculate service area coverage
- Vendor service area management

### **Phase 3: Background Jobs (Hours 20-21)**

**19. Scheduled Tasks (Hangfire) ✅**
- **DeliveryGeneratorJob** (daily 12:00 AM)
  - Generate delivery records for next day
  - Check subscription frequency
  - Create Scheduled deliveries
  - Include one-time items for delivery date

- **InvoiceGeneratorJob** (daily 2:00 AM)
  - Find subscriptions where NextBillingDate <= Today
  - Generate invoices for unpaid deliveries
  - Update NextBillingDate
  - Send notifications

- **OverdueInvoiceCheckerJob** (daily 6:00 AM)
  - Mark invoices past due date as Overdue
  - Send payment reminders

- **SubscriptionStatusCheckerJob** (daily 10:00 AM)
  - Check unpaid amounts vs credit limits
  - Auto-suspend if exceeded
  - Auto-resume if paid

- **OneTimeDeliveryCleanupJob** (daily 11:00 PM)
  - Auto-cancel completed one-time subscriptions
  - Mark as Settled

### **Phase 4: Polish & Deploy (Hours 21-24)**

**20. Testing ✅**
- Unit tests for business logic
- Integration tests for API endpoints
- Test critical paths

**21. Documentation ✅**
- Swagger/OpenAPI docs
- README with setup instructions
- API endpoint documentation
- Database schema diagram
- Deployment guide

**22. Error Handling & Logging ✅**
- Global exception handler
- Validation error responses
- Structured logging (Serilog)
- Request/response logging

**23. Deployment ✅**
- Azure App Service (API)
- Azure SQL Database
- Azure Blob Storage (files)
- CI/CD with GitHub Actions
- Environment-based configuration

**24. Demo Data Seeding ✅**
- Sample categories
- Sample products
- Test vendor accounts
- Sample subscriptions

---

## 🚀 Go-to-Market Strategy

### **Phase 1: Vendor Acquisition (Month 1-2)**

**Target:** 10 vendors in one locality (Koramangala, Bangalore)

**Vendor Profile:**
- Traditional dairy vendors (5 vendors)
- Water suppliers (3 vendors)
- Newspaper distributors (2 vendors)
- Each serving 50+ existing customers

**Sales Process:**
1. Identify high-volume vendors
2. Demo: Show ROI (payment processing, customer management)
3. Onboard: Set up vendor account, products, service areas
4. Train: How to add customers, manage deliveries
5. Support: Dedicated WhatsApp group for queries

**Success Criteria:**
- 10 vendors onboarded
- Each adds 30+ customers (300-500 total customers)
- 60-70% invitation acceptance rate
- Network density in one locality

### **Phase 2: Customer Migration (Month 2-3)**

**Vendor-Led Onboarding:**
- Each vendor invites existing customers via app
- ShopCore sends SMS/WhatsApp with invitation link
- Customer one-click acceptance
- Account auto-created with pre-filled subscription

**Expected Results:**
- 10 vendors × 50 customers = 500 invitations sent
- 60-70% acceptance = 300-350 active customers
- Monthly billing activated
- Positive cash flow from commission

### **Phase 3: Organic Growth (Month 3+)**

**Once Density Achieved:**
- Word of mouth (neighbors see delivery person)
- Customer discovery works (search finds vendors nearby)
- Cross-vendor adoption (customer adds second vendor)
- Vendor referrals (vendors invite other vendors)

**Growth Mechanisms:**
- Customer invites friends (referral program)
- Vendor expands service areas
- New vendors join seeing success
- Network effects kick in

---

## 💰 Business Model

### **Revenue Streams**

**1. Vendor Commission (Primary - 80%):**
```
Per Transaction:
- E-commerce order: 5% commission
- Subscription delivery: 5% commission
- Example: ₹100 order → ₹5 commission

Monthly (per vendor with 50 customers):
- 50 customers × ₹1800/month = ₹90,000 GMV
- Commission: ₹90,000 × 5% = ₹4,500/month
- 10 vendors = ₹45,000/month
```

**2. Premium Vendor Features (10%):**
```
- Featured listing: ₹500/month
- Priority in search: ₹300/month
- Advanced analytics: ₹200/month
- Targeted promotions: ₹500/month
```

**3. Customer Convenience Fee (10%):**
```
- Express delivery: ₹20 per order
- Weekend delivery: ₹30 per order
- Late night delivery: ₹50 per order
```

### **Unit Economics**

**Subscription Customer:**
```
Monthly Revenue: ₹1,800
Commission (5%): ₹90
Payment Processing (monthly): ₹20 (1.1%)
Net Revenue: ₹70/customer/month
```

**E-commerce Customer:**
```
Order Value: ₹300
Commission (5%): ₹15
Payment Processing (per order): ₹6 (2%)
Net Revenue: ₹9/order
To equal subscription: Needs 8 orders/month
```

**Why Subscription is Better:**
- 8x better economics
- Lower payment fees (1 transaction/month vs 30)
- Higher customer lifetime value
- Lower churn
- Predictable revenue

### **Cost Structure**

**Fixed Costs:**
- Engineering team: ₹3,00,000/month (2 developers)
- Sales team: ₹1,50,000/month (2 sales reps)
- Operations: ₹50,000/month
- Infrastructure (Azure): ₹20,000/month
- Total Fixed: ₹5,20,000/month

**Variable Costs:**
- Payment gateway: 2% of GMV
- SMS/WhatsApp: ₹0.10 per message
- Customer support: ₹50/customer/month

**Break-Even:**
- Need: ₹5,20,000 / ₹70 per customer = 7,429 active customers
- Or: 150 vendors × 50 customers each
- Timeline: Month 6-8 (realistic)

---

## 🎯 Success Metrics

### **After 24 Hours (MVP Complete):**

**Technical:**
- ✅ 85+ API endpoints working
- ✅ 23 database tables properly related
- ✅ Clean Architecture implemented
- ✅ CQRS pattern with MediatR
- ✅ JWT authentication functional
- ✅ Payment integration working
- ✅ Background jobs scheduled

**Business:**
- ✅ Multi-vendor order splitting logic
- ✅ Subscription with flexible billing
- ✅ Deposit system with settlement
- ✅ Vendor-led customer onboarding
- ✅ Location-based vendor discovery

**Quality:**
- ✅ Swagger documentation complete
- ✅ Unit tests for critical paths
- ✅ Error handling comprehensive
- ✅ Public version deployed (GitHub)
- ✅ Demo data available

### **After Month 1 (Beta Launch):**

- 10 vendors onboarded
- 300+ active customers
- ₹5,00,000+ GMV
- 95%+ uptime
- <100ms average API response
- <1% payment failure rate

### **After Month 3 (Market Validation):**

- 50 vendors onboarded
- 2,000+ active customers
- ₹30,00,000+ GMV
- Break-even approaching
- Product-market fit signals
- Ready for seed funding

### **After Month 6 (Scale):**

- 150 vendors onboarded
- 7,500+ active customers
- ₹1,35,00,000+ GMV
- Break-even achieved
- Expand to 3 localities
- Series A preparation

---

## 📋 API Endpoint Summary

### **Total: ~90 Endpoints**

**Public (15):**
- Auth: 7 (register, login, logout, reset password, verify email, refresh token, forgot password)
- Categories: 4 (list, get, search, products)
- Products: 4 (list, get, search, featured)

**Customer (32):**
- User: 4 (profile, update, avatar, change password)
- Addresses: 5 (list, add, update, delete, set default)
- Cart: 6 (get, add, update, remove, clear, validate)
- Orders: 5 (create, list, get, cancel, cancel item, invoice)
- Reviews: 5 (list, create, update, delete, mark helpful)
- Wishlist: 3 (list, add, remove)
- Payments: 4 (create intent, confirm, history, refund)

**Vendor (18):**
- Profile: 4 (register, get, update, stats, upload logo)
- Service Areas: 3 (add, update, remove)
- Products: 8 (create, update, delete, images, status, specs)
- Orders: 3 (list, get, update item status)
- Invitations: 4 (create, list, resend, cancel)

**Subscription (18) - PRIVATE:**
- Subscriptions: 9 (create, list, get, update, pause, resume, cancel, settle, add one-time item)
- Deliveries: 5 (list, get, complete, skip, failed)
- Invoices: 4 (list, get, pay, generate, download)

**Admin (12):**
- Vendors: 3 (list pending, approve, suspend)
- Categories: 3 (create, update, delete)
- Coupons: 3 (create, list, deactivate)
- Payouts: 2 (calculate, process)
- Dashboard: 1 (stats)

**Location (3):**
- Search: 1 (search vendors by location)
- Geocoding: 1 (geocode address)
- Service Areas: 1 (check coverage)

---

## 🔒 Repository Strategy

### **Public Repo: shopcore-api (GitHub)**

**Contains:** E-commerce features only (Phases 1, 3, 4)

**Excluded via .gitignore:**
```
# Subscription features (private)
**/Features/Subscriptions/
**/Features/Deliveries/
**/Features/Invoices/
**/Features/CustomerInvitations/

# Subscription entities
**/Entities/Subscription.cs
**/Entities/SubscriptionItem.cs
**/Entities/Delivery.cs
**/Entities/DeliveryItem.cs
**/Entities/SubscriptionInvoice.cs
**/Entities/CustomerInvitation.cs

# Subscription controllers
**/Controllers/SubscriptionsController.cs
**/Controllers/DeliveriesController.cs
**/Controllers/InvoicesController.cs

# Private frontend
frontend/shopcore-subscribe/
```

**README Focus:**
- Multi-vendor e-commerce API
- Clean Architecture showcase
- CQRS with MediatR
- Payment integration
- Order management
- Vendor dashboard

### **Private Repo: shopcore-complete**

**Contains:** Everything (all phases)
- Full e-commerce system
- Complete subscription system
- Both frontends
- All documentation

**Purpose:**
- Real business deployment
- Competitive moat features
- Trade secrets protection

---

## 🎓 Learning Outcomes

This project demonstrates:

1. **Clean Architecture** - Proper separation of concerns
2. **CQRS Pattern** - Command/Query separation with MediatR
3. **Domain-Driven Design** - Rich domain models
4. **Multi-tenancy** - Vendor isolation and data security
5. **Complex Business Logic** - Multi-vendor orders, subscriptions
6. **Payment Integration** - Real payment processing with refunds
7. **Background Jobs** - Scheduled task execution
8. **Location Services** - Geolocation, service areas
9. **Testing** - Unit and integration tests
10. **API Design** - RESTful, documented, versioned
11. **Production Deployment** - Azure hosting, CI/CD
12. **Product Thinking** - Real business model, go-to-market strategy

---

## 🚀 Next Steps

### **Immediate (Hours 0-4):**
1. Complete project setup (solution structure, packages)
2. Create all domain entities (23 tables)
3. Configure entity relationships (EF Core)
4. Create initial migration
5. Seed basic data

### **Short-term (Hours 4-20):**
1. Implement all commands/handlers (CQRS)
2. Create validators (FluentValidation)
3. Build controllers (API layer)
4. Add background jobs (Hangfire)
5. Write tests (unit + integration)

### **Medium-term (Hours 20-30):**
1. Deploy public version (GitHub + Azure)
2. Create public frontend (e-commerce UI)
3. Record demo video
4. Write impressive README
5. Start job applications

### **Long-term (Month 2+):**
1. Build private frontend (subscription UI)
2. Add Phase 2 features (Driver, SubAdmin roles)
3. Master product catalog system
4. Route optimization
5. Real-time tracking
6. Launch in market

---

## 📌 Critical Architectural Decisions

### **1. Order Status is Derived (Not Set Directly)**
- Order.Status computed from OrderItem statuses
- Vendors update their items independently
- Order status auto-updates via UpdateStatusFromItems()
- Handles multi-vendor complexity cleanly

### **2. Subscriptions ≠ Orders**
- Completely separate entities
- Different status lifecycles
- Different payment models
- Different user flows
- No mixing of concepts

### **3. One-Time Items = Subscription Items**
- One-time items are SubscriptionItems with IsRecurring = false
- Piggyback on regular deliveries
- Same vendor, same billing cycle
- Keeps architecture consistent

### **4. Vendor-Led Onboarding is Primary**
- 90% of customers come via vendor invitations
- 10% via customer discovery
- Leverages existing relationships
- Viral growth mechanism

### **5. Location is Critical**
- Vendor service areas define coverage
- Pincode-based matching
- Geolocation for addresses
- Foundation for future routing

### **6. Deposit System is Optional**
- Vendor chooses if required
- Held throughout subscription
- Settled only at cancellation
- Never touched during normal operations

### **7. Feature Flags for Deployment**
- Same backend, different features enabled
- Public deployment: Subscriptions disabled
- Private deployment: Everything enabled
- Easy to maintain both versions

---

## ⚠️ Known Trade-offs

### **1. No Driver/SubAdmin Roles (Yet)**
- Deferred to Phase 2
- Adds complexity without immediate value
- Can add after MVP validation

### **2. No Master Product Catalog (Yet)**
- Will build after market learning
- Let vendors teach us what's needed
- Can add in Month 2-3

### **3. No Real-time Tracking (Yet)**
- Requires GPS integration
- Complex infrastructure
- Phase 2 feature

### **4. Basic Route Optimization**
- Manual vendor routing initially
- Can optimize later with ML
- Good enough for MVP

### **5. Single Payment Gateway**
- Start with Razorpay (India)
- Can add Stripe later (international)
- Reduces initial complexity

---

## 🎯 Competitive Advantages

**vs Blinkit/Zepto (Quick Commerce):**
- ✅ We don't compete on speed (10 min delivery)
- ✅ We win on subscription economics (90% cheaper)
- ✅ We target daily essentials (they target impulse)
- ✅ We have vendor relationships (they have dark stores)

**vs BigBasket:**
- ✅ We focus on subscriptions (they're grocery-first)
- ✅ We have deposit system (they don't)
- ✅ We have vendor-led onboarding (they don't)
- ✅ We're local vendor platform (they're aggregator)

**vs Country Delight/Milkbasket:**
- ✅ We're multi-vendor (they're single brand)
- ✅ We have flexible billing (they're fixed)
- ✅ We have deposits (they don't)
- ✅ We're platform (they're product company)

**Unique Moat:**
- Deposit system with smart settlement
- Flexible billing cycles (daily delivery, monthly payment)
- Multi-item subscriptions (one relationship, many products)
- Vendor-led distribution (viral growth)
- One-time additions to subscriptions (flexibility)

---

**Built with ❤️ for the Indian local vendor ecosystem**

**Goal:** Get hired in 4-6 weeks, launch product in 3-4 months! 🚀
