# ShopCore API - Project Context Document

**Version:** 1.0  
**Last Updated:** January 24, 2026  
**Development Approach:** 24-Hour Hackathon Build

---

## 🎯 Project Overview

**ShopCore** is a multi-vendor e-commerce platform API with an innovative subscription-based recurring delivery system for daily essentials (milk, water, groceries, vegetables).

### **Target Market**
- **Geography:** India
- **Vendors:** Local kirana stores, dairy vendors, water suppliers, vegetable sellers
- **Customers:** B2C households and B2B offices

### **Unique Value Proposition**
1. **Flexible Payment Cycles:** Subscribe daily, pay monthly/weekly (90% lower payment processing fees)
2. **Multi-Vendor Support:** Single platform for all local vendors
3. **Optional Deposit System:** Security for vendors, settled at subscription end
4. **Auto-Calculated Billing:** Smart billing cycles based on deposit amount

---

## 🏗️ Architecture

### **Pattern:** Clean Architecture with CQRS
```
ShopCore.API          (Web API layer)
    ↓
ShopCore.Application  (Business logic, CQRS handlers)
    ↓
ShopCore.Domain       (Entities, interfaces)
    ↓
ShopCore.Infrastructure (EF Core, external services)
```

### **Tech Stack**
- **.NET 8** Web API
- **Entity Framework Core 8** (Code-First)
- **SQL Server** (LocalDB dev, Azure SQL prod)
- **MediatR** (CQRS pattern)
- **JWT Bearer** authentication
- **FluentValidation** (input validation)
- **AutoMapper** (object mapping)
- **Swagger/OpenAPI** (documentation)

### **External Services**
- **Payment:** Razorpay (India) / Stripe (International)
- **Email:** SendGrid / SMTP
- **Storage:** Local filesystem (dev) / Azure Blob (prod)

---

## 📊 Database Schema

### **Public E-Commerce Tables (16)**
1. User
2. VendorProfile
3. Address
4. Category
5. Product
6. ProductImage
7. ProductSpecification
8. Cart
9. CartItem
10. Order
11. OrderItem
12. OrderStatusHistory
13. Review
14. VendorPayout
15. Coupon
16. Wishlist

### **Private Subscription Tables (3)**
17. Subscription
18. Delivery
19. SubscriptionInvoice

**Total: 19 Tables**

---

## 🎯 Feature List (Implementation Order)

### **PHASE 1: Foundation (Hours 0-4)**

#### 1. Authentication & Authorization
- User registration (email, password, phone, role)
- User login (JWT token generation)
- Logout (invalidate refresh token)
- Password reset (email-based)
- Email verification
- Role-based authorization: Customer, Vendor, Admin

#### 2. User Management
- Get current user profile
- Update profile (name, phone)
- Upload profile picture
- Change password

#### 3. Address Management
- List user addresses
- Add new address (full address with pincode)
- Update address
- Delete address
- Set default address

---

### **PHASE 2: Vendor & Catalog (Hours 4-8)**

#### 4. Vendor Management
- Register as vendor (business name, GST, PAN, bank details)
- Vendor profile (get/update)
- Vendor statistics (products, orders, revenue)
- List vendor orders
- Update order status
- **Admin:** Approve/suspend vendors

#### 5. Category Management
- List categories (hierarchical tree)
- Get category details
- Get products in category
- **Admin:** Create/update/delete categories

**Sample Categories:**
```
Beverages → Water, Milk, Juice
Groceries → Rice, Flour, Pulses
Dairy → Milk, Paneer, Butter
Vegetables → Leafy, Root, Seasonal
```

#### 6. Product Management
- List products (pagination, filters, sorting)
- Get product details
- Search products (full-text)
- Get featured products
- **Vendor:** Create product
- **Vendor:** Update product
- **Vendor:** Delete product
- **Vendor:** Upload product images (multiple)
- **Vendor:** Update product status (Active/OutOfStock/Discontinued)

**Product Fields:**
```
Basic: name, slug, description, price, compareAtPrice
Inventory: stockQuantity, SKU, trackInventory
Subscription: isSubscriptionAvailable, subscriptionDiscount
Vendor: requiresDeposit, defaultDepositAmount
SEO: metaTitle, metaDescription
Stats: viewCount, orderCount, averageRating
```

---

### **PHASE 3: Shopping & Orders (Hours 8-12)**

#### 7. Shopping Cart
- Get cart (with totals)
- Add to cart (validate stock)
- Update cart item quantity
- Remove from cart
- Clear cart
- Validate cart (before checkout)

**Business Logic:**
- Auto-update prices if changed
- Remove out-of-stock items
- Calculate totals with 18% GST

#### 8. Order Management
- Create order (checkout from cart)
- Multi-vendor order splitting
- List user orders (filter, pagination)
- Get order details
- Cancel order (if Pending/Confirmed)
- Download invoice (PDF)
- **Vendor:** Update order status

**Order Status Flow:**
```
Pending → Confirmed → Processing → Shipped → Delivered
    ↓
Cancelled → Refunded
```

**Multi-Vendor Logic:**
- Single Order with multiple OrderItems
- Each OrderItem has vendorId
- Independent status per vendor
- Commission calculated per item

#### 9. Payment Integration
- Create payment intent (Razorpay/Stripe)
- Confirm payment
- Payment webhook
- Get payment history
- Support: Online payment + Cash on Delivery (COD)

---

### **PHASE 4: Enhancements (Hours 12-16)**

#### 10. Review & Rating System
- Get product reviews (pagination, sorting)
- Create review (1-5 stars, only for purchased products)
- Update review
- Delete review
- Mark review helpful
- **Vendor:** Respond to review

#### 11. Wishlist
- Get wishlist
- Add to wishlist
- Remove from wishlist
- Move to cart

#### 12. Discount Coupons
- List active coupons
- Validate coupon code
- Apply coupon to cart
- **Admin:** Create coupon
- **Admin:** Deactivate coupon

**Coupon Types:**
- Percentage (10% off)
- Fixed amount (₹100 off)
- Free shipping
- Minimum order requirements

#### 13. Vendor Payouts
- Calculate vendor earnings
- Track commission deductions
- Generate payout reports
- **Admin:** Process payouts

#### 14. Notifications
- Email notifications for:
  - Order confirmation
  - Order status updates
  - Vendor approval
  - Payment receipts
  - Review reminders

---

### **PHASE 5: Subscription System (Hours 16-20) - PRIVATE**

#### 15. Subscription Management
- Create subscription (product, quantity, frequency, start date)
- List user subscriptions (filter by status)
- Get subscription details
- Update subscription (quantity, frequency)
- Pause subscription
- Resume subscription
- Cancel subscription (with settlement)

**Subscription Frequencies:**
```
Daily, EveryTwoDays, Weekly, BiWeekly, Monthly, Custom
```

**Deposit System:**
```
Optional: Vendor sets requirement and amount
Payment: One-time at subscription start
Usage: Safety buffer, settled at subscription end
Settlement: Calculate total owed - total paid - deposit
    If positive: Customer pays vendor
    If negative: Vendor refunds customer
    If zero: All settled
```

**Billing Cycle Logic:**
```
Option 1: Vendor sets explicit cycle (e.g., "Bill every 30 days")
Option 2: Auto-calculate from deposit
    deposit_amount / daily_price = billing_cycle_days
    Example: ₹600 / ₹20 = 30 days
    Round down for safety
```

#### 16. Delivery Management
- List upcoming deliveries
- Get delivery details
- Mark delivery complete (by vendor/driver)
- Skip delivery (by customer)
- Reschedule failed delivery
- Payment on delivery (COD/UPI)

**Delivery Status:**
```
Scheduled → OutForDelivery → Delivered / Failed / Skipped
```

**Payment Status:**
```
Unpaid, Paid, Pending (independent of delivery status)
```

#### 17. Invoice Management
- Auto-generate invoices (cron job based on billing cycle)
- Manual invoice generation (vendor request)
- Get unpaid invoices
- Pay invoice (online payment)
- Download invoice (PDF)

**Invoice Generation Logic:**
```
Query: All delivered + unpaid deliveries where InvoiceId IS NULL
Create invoice grouping these deliveries
Set InvoiceId on deliveries
Send notification to customer
```

#### 18. Subscription Settlement
- "Settle Account" button
- Calculate final balance
- Handle refund/payment
- Close subscription

**Settlement Calculation:**
```
total_delivered = SUM(all deliveries)
total_paid = SUM(all payments)
net_balance = total_delivered - total_paid - deposit

If net_balance > 0: Customer owes vendor
If net_balance < 0: Vendor refunds customer
If net_balance = 0: Settled
```

---

### **PHASE 6: Background Jobs (Hours 20-21)**

#### 19. Scheduled Tasks (Hangfire/Quartz)
- **DeliveryGeneratorJob** (runs daily 12:00 AM)
  - Generate delivery records for next day
  - Check subscription frequency
  - Create Scheduled deliveries

- **InvoiceGeneratorJob** (runs daily 2:00 AM)
  - Find subscriptions where NextBillingDate <= Today
  - Generate invoices for unpaid deliveries
  - Update NextBillingDate

- **OverdueInvoiceCheckerJob** (runs daily 6:00 AM)
  - Mark invoices past due date as Overdue
  - Send payment reminders

- **SubscriptionStatusCheckerJob** (runs daily 10:00 AM)
  - Check unpaid amounts vs credit limits
  - Auto-suspend if exceeded
  - Auto-resume if paid

---

### **PHASE 7: Polish & Deploy (Hours 21-24)**

#### 20. Testing
- Unit tests for business logic
- Integration tests for API endpoints
- Test critical paths:
  - User registration → Login
  - Product creation → Add to cart → Checkout
  - Subscription → Delivery → Invoice → Payment

#### 21. Documentation
- Swagger/OpenAPI docs
- README with setup instructions
- API endpoint documentation
- Database schema diagram
- Deployment guide

#### 22. Error Handling & Logging
- Global exception handler
- Validation error responses
- Structured logging (Serilog)
- Request/response logging

#### 23. Deployment
- Azure App Service (API)
- Azure SQL Database
- Azure Blob Storage (files)
- CI/CD with GitHub Actions
- Environment-based configuration

#### 24. Demo Data Seeding
- Sample categories
- Sample products
- Test vendor accounts
- Sample subscriptions

---

## 🔄 API Endpoint Summary

### **Total: ~80 Endpoints**

**Public (15):**
- Auth: 7 (register, login, logout, reset password, verify email)
- Categories: 4 (list, get, search, products)
- Products: 4 (list, get, search, featured)

**Customer (28):**
- User: 5 (profile, update, avatar, change password)
- Addresses: 5 (list, add, update, delete, set default)
- Cart: 6 (get, add, update, remove, clear, validate)
- Orders: 5 (create, list, get, cancel, invoice)
- Reviews: 4 (list, create, update, delete)
- Wishlist: 3 (list, add, remove)

**Vendor (15):**
- Profile: 4 (register, get, update, stats)
- Products: 8 (create, update, delete, images, status)
- Orders: 3 (list, get, update status)

**Subscription (15) - PRIVATE:**
- Subscriptions: 8 (create, list, get, update, pause, resume, cancel, settle)
- Deliveries: 5 (list, get, complete, skip, reschedule)
- Invoices: 2 (list, pay)

**Admin (12):**
- Vendors: 3 (list pending, approve, suspend)
- Categories: 3 (create, update, delete)
- Coupons: 3 (create, list, deactivate)
- Payouts: 2 (calculate, process)
- Dashboard: 1 (stats)

---

## 🎯 Critical Business Rules

### **Order Rules**
1. Multi-vendor orders split automatically
2. Commission calculated per OrderItem
3. Each vendor manages their portion independently
4. Order can only be cancelled if Pending/Confirmed

### **Subscription Rules**
1. Deliveries happen regardless of payment status
2. Delivery status and payment status are independent
3. Invoice only includes deliveries NOT already invoiced (InvoiceId IS NULL)
4. Unpaid amount accumulates until invoice generated
5. Credit limit = protection threshold for auto-suspension
6. Deposit is settled only when subscription ends

### **Payment Rules**
1. Payment on delivery updates that delivery immediately
2. Invoice payment updates all linked deliveries
3. Deposit never touched during normal operations
4. Settlement happens only at subscription cancellation

### **Billing Cycle Rules**
1. Vendor can set explicit cycle (e.g., 30 days)
2. OR system auto-calculates: deposit / daily_price (rounded down)
3. Next billing date calculated from last invoice date
4. Manual invoice generation doesn't affect automatic schedule

---

## 🚀 Implementation Priority

### **Day 1-2: PUBLIC VERSION (GitHub)**
✅ Hours 0-16: Complete e-commerce platform
- Authentication, users, addresses
- Vendors, categories, products
- Cart, orders, payments
- Reviews, wishlist, coupons
- Vendor payouts

**Result:** Impressive portfolio project for job applications

### **Day 3-4: PRIVATE VERSION (Local Only)**
🔒 Hours 16-24: Add subscription features
- Subscriptions, deliveries, invoices
- Background jobs
- Deposit system
- Settlement logic

**Result:** Complete product with competitive moat

---

## 📝 Development Checklist

### **Setup (30 minutes)**
- [ ] Create solution structure (Clean Architecture)
- [ ] Install NuGet packages
- [ ] Configure appsettings.json
- [ ] Setup DbContext and entities
- [ ] Initial migration

### **Phase 1: Foundation (4 hours)**
- [ ] Authentication system
- [ ] User management
- [ ] Address management
- [ ] Basic middleware (auth, error handling)

### **Phase 2: Catalog (4 hours)**
- [ ] Vendor management
- [ ] Category CRUD
- [ ] Product CRUD
- [ ] Image upload
- [ ] Search functionality

### **Phase 3: Shopping (4 hours)**
- [ ] Shopping cart
- [ ] Order creation
- [ ] Multi-vendor splitting
- [ ] Payment integration
- [ ] Order status management

### **Phase 4: Enhancements (4 hours)**
- [ ] Review system
- [ ] Wishlist
- [ ] Coupons
- [ ] Vendor payouts
- [ ] Email notifications

**CHECKPOINT: Public version complete → Deploy to GitHub**

### **Phase 5: Subscriptions (4 hours)**
- [ ] Subscription CRUD
- [ ] Delivery management
- [ ] Invoice generation
- [ ] Deposit system
- [ ] Settlement logic

### **Phase 6: Automation (1 hour)**
- [ ] Background jobs (Hangfire)
- [ ] Delivery scheduler
- [ ] Invoice generator
- [ ] Status checker

### **Phase 7: Polish (3 hours)**
- [ ] Unit tests
- [ ] Integration tests
- [ ] Swagger documentation
- [ ] Error handling
- [ ] Logging
- [ ] Demo data seeding
- [ ] Deploy to Azure

---

## 🎯 Success Metrics

After 24 hours:

**Technical:**
- ✅ 80+ API endpoints working
- ✅ 19 database tables properly related
- ✅ Clean Architecture implemented
- ✅ CQRS pattern with MediatR
- ✅ JWT authentication functional
- ✅ Payment integration working
- ✅ Background jobs scheduled

**Business:**
- ✅ Multi-vendor order splitting logic
- ✅ Subscription with flexible billing
- ✅ Deposit system with settlement
- ✅ Auto-calculated billing cycles

**Quality:**
- ✅ Swagger documentation complete
- ✅ Unit tests for critical paths
- ✅ Error handling comprehensive
- ✅ Deployed and accessible
- ✅ Demo data available

---

## 📦 Repository Strategy

### **Public Repo: "shopcore-api"**
**Contains:** Phases 1-4 (e-commerce only)
**Purpose:** Portfolio project for job applications
**Visibility:** Public on GitHub

### **Private Repo: "shopcore-complete"**
**Contains:** All phases (e-commerce + subscriptions)
**Purpose:** Actual product with competitive moat
**Visibility:** Private local repository

**Strategy:**
1. Build publicly (phases 1-4)
2. Get hired
3. Add subscription features privately
4. Launch product with salary backing

---

## 🎓 Learning Outcomes

This project demonstrates:

1. **Clean Architecture** - Proper separation of concerns
2. **CQRS Pattern** - Command/Query separation with MediatR
3. **Domain-Driven Design** - Rich domain models
4. **Multi-tenancy** - Vendor isolation and data security
5. **Complex Business Logic** - Multi-vendor orders, subscriptions
6. **Payment Integration** - Real payment processing
7. **Background Jobs** - Scheduled task execution
8. **Testing** - Unit and integration tests
9. **API Design** - RESTful, documented, versioned
10. **Production Deployment** - Azure hosting, CI/CD

---

## 🚀 Quick Start Commands

```bash
# Clone and setup
git clone <repo-url>
cd ShopCore

# Restore packages
dotnet restore

# Update connection string in appsettings.json
# Then create database
dotnet ef database update --project ShopCore.Infrastructure

# Run
dotnet run --project ShopCore.API

# Navigate to
https://localhost:7001/swagger
```

---

## 🎯 Next Steps After Completion

### **For Job Search:**
1. Push public version to GitHub
2. Create impressive README with screenshots
3. Record demo video (3-5 minutes)
4. Update LinkedIn with project
5. Add to resume
6. Start applying (target: 15-20 applications/day)

### **For Product Launch:**
1. Add subscription features (private)
2. Test with 5 beta vendors
3. Iterate based on feedback
4. Launch in single city (Bangalore)
5. Acquire first paying customers
6. Scale to other cities

---

**Built with ❤️ for the Indian local vendor ecosystem**

**Goal:** Get hired in 4-6 weeks, launch product in 3-4 months! 🚀
