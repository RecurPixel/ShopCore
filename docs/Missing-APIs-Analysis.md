# Missing APIs Analysis for ShopCore

## Analysis Date: February 2026

After comprehensive review of requirements, handler implementations, and existing API routes, here are the **MISSING APIs** that should be added:

---

## ❌ CRITICAL MISSING APIs

### 1. **Notification System**
**Status:** Completely Missing  
**Required Endpoints:**

```http
# User Notifications
GET    /users/me/notifications              # List all notifications
GET    /users/me/notifications/unread       # Get unread count
PATCH  /users/me/notifications/{id}/read    # Mark as read
PATCH  /users/me/notifications/read-all     # Mark all as read
DELETE /users/me/notifications/{id}         # Delete notification
PUT    /users/me/notification-preferences   # Update preferences

# Vendor Notifications
GET    /vendors/me/notifications             # Vendor-specific notifications
```

**Use Cases:**
- Order status updates
- New subscription created
- Payment received
- Delivery scheduled
- Low stock alerts
- Review posted
- Vendor approval status

---

### 2. **Product Search & Filters**
**Status:** Partially Missing  
**Missing Endpoints:**

```http
GET    /products/search?q={query}                    # ✅ Exists
GET    /products/filters                             # ❌ Get available filters
GET    /products/autocomplete?q={query}              # ❌ Search suggestions
GET    /products/similar/{id}                        # ❌ Similar products
GET    /products/recently-viewed                     # ❌ User's recent views
GET    /products/trending                            # ❌ Trending products
```

**Missing Query Parameters for /products:**
- `minPrice`, `maxPrice` - Price range filter
- `inStock` - Only show in-stock items
- `vendorId` - Filter by vendor
- `hasDiscount` - Items on discount
- `minRating` - Minimum rating filter
- `sortBy` - (price, rating, newest, popular)

---

### 3. **Review System Enhancements**
**Status:** Missing Advanced Features  
**Missing Endpoints:**

```http
POST   /products/{productId}/reviews/{reviewId}/helpful    # ✅ Mentioned
POST   /products/{productId}/reviews/{reviewId}/report     # ❌ Report review
GET    /products/{productId}/reviews/summary               # ❌ Rating distribution
POST   /products/{productId}/reviews/{reviewId}/images     # ❌ Upload review images

# Vendor Response
POST   /vendors/me/reviews/{reviewId}/respond              # ✅ Mentioned but needs detail
PUT    /vendors/me/reviews/{reviewId}/response             # ❌ Edit response
DELETE /vendors/me/reviews/{reviewId}/response             # ❌ Delete response
```

---

### 4. **Order Tracking & Returns**
**Status:** Missing Critical Features  
**Missing Endpoints:**

```http
# Order Tracking
GET    /orders/{id}/tracking                               # ❌ Real-time tracking
GET    /orders/{id}/timeline                               # ❌ Detailed status history

# Returns & Refunds
POST   /orders/{id}/return-request                         # ❌ Request return
GET    /users/me/returns                                    # ❌ List return requests
GET    /vendors/me/returns                                  # ❌ Vendor return requests
PATCH  /vendors/me/returns/{id}/approve                     # ❌ Approve return
POST   /orders/{id}/refund                                  # ❌ Process refund

# Customer Support
POST   /orders/{id}/dispute                                 # ❌ Raise dispute
GET    /users/me/disputes                                   # ❌ List disputes
```

---

### 5. **Subscription Management - Missing Features**
**Status:** Core Features Missing  
**Missing Endpoints:**

```http
# Subscription Modifications
POST   /subscriptions/{id}/modify-schedule                  # ❌ Change delivery days
POST   /subscriptions/{id}/skip-dates                       # ❌ Skip multiple dates
POST   /subscriptions/{id}/vacation-mode                    # ❌ Pause for vacation
GET    /subscriptions/{id}/upcoming-deliveries              # ❌ Next 30 days

# Subscription Analytics
GET    /users/me/subscriptions/analytics                    # ❌ Spending analytics
GET    /vendors/me/subscriptions/analytics                  # ❌ Revenue analytics
GET    /vendors/me/subscriptions/churn-risk                 # ❌ At-risk customers

# One-Time Additions
POST   /subscriptions/{id}/one-time-items                   # ❌ Add one-time item
GET    /subscriptions/{id}/one-time-items                   # ❌ List one-time items
DELETE /subscriptions/{id}/one-time-items/{itemId}          # ❌ Remove one-time item
```

---

### 6. **Delivery Management - Missing Features**
**Status:** Critical Features Missing  
**Missing Endpoints:**

```http
# Customer Delivery Management
POST   /deliveries/{id}/reschedule                          # ❌ Customer reschedule
POST   /deliveries/{id}/delivery-instructions               # ❌ Add instructions
GET    /deliveries/{id}/eta                                 # ❌ Estimated time
POST   /deliveries/{id}/feedback                            # ❌ Rate delivery

# Vendor Delivery Routing
GET    /vendors/me/deliveries/route?date={date}             # ❌ Optimized route
POST   /vendors/me/deliveries/batch-update                  # ❌ Bulk status update
GET    /vendors/me/deliveries/summary?date={date}           # ❌ Daily summary

# Delivery Proof & Issues
POST   /deliveries/{id}/report-issue                        # ❌ Report problem
GET    /deliveries/{id}/proof                               # ❌ Get delivery proof
```

---

### 7. **Payment Management - Missing Features**
**Status:** Critical Features Missing  
**Missing Endpoints:**

```http
# Payment Methods
GET    /users/me/payment-methods                            # ❌ Saved cards/UPI
POST   /users/me/payment-methods                            # ❌ Add payment method
DELETE /users/me/payment-methods/{id}                       # ❌ Remove method
PATCH  /users/me/payment-methods/{id}/default               # ❌ Set default

# Payment History & Receipts
GET    /payments/{id}/receipt                               # ❌ Download receipt
POST   /payments/{id}/retry                                 # ❌ Retry failed payment

# Wallet/Credits (Future)
GET    /users/me/wallet                                     # ❌ Wallet balance
POST   /users/me/wallet/add-money                           # ❌ Add to wallet
GET    /users/me/wallet/transactions                        # ❌ Wallet history
```

---

### 8. **Vendor Payout System - Missing Details**
**Status:** Needs Expansion  
**Missing Endpoints:**

```http
GET    /vendors/me/payouts/breakdown                        # ❌ Detailed breakdown
POST   /vendors/me/payouts/request                          # ❌ Request payout
GET    /vendors/me/payouts/{id}/details                     # ❌ Payout details
GET    /vendors/me/payouts/{id}/invoice                     # ❌ Payout invoice

# Admin Payout Management
GET    /admin/payouts/pending                               # ❌ Pending payouts
POST   /admin/payouts/{id}/approve                          # ❌ Approve payout
POST   /admin/payouts/{id}/reject                           # ❌ Reject with reason
GET    /admin/payouts/schedule                              # ❌ Payout schedule
```

---

### 9. **Analytics & Reports**
**Status:** Completely Missing  
**Required Endpoints:**

```http
# Customer Analytics
GET    /users/me/analytics/spending                         # ❌ Spending trends
GET    /users/me/analytics/orders                           # ❌ Order history analytics
GET    /users/me/analytics/subscriptions                    # ❌ Subscription analytics

# Vendor Analytics
GET    /vendors/me/analytics/dashboard                      # ❌ Main dashboard
GET    /vendors/me/analytics/sales                          # ❌ Sales trends
GET    /vendors/me/analytics/products                       # ❌ Product performance
GET    /vendors/me/analytics/customers                      # ❌ Customer insights
GET    /vendors/me/analytics/deliveries                     # ❌ Delivery metrics

# Admin Analytics
GET    /admin/analytics/platform-stats                      # ❌ Platform overview
GET    /admin/analytics/revenue                             # ❌ Revenue reports
GET    /admin/analytics/vendors                             # ❌ Vendor performance
GET    /admin/analytics/customers                           # ❌ Customer metrics
GET    /admin/analytics/products                            # ❌ Product analytics
```

---

### 10. **Customer Invitations - Needs Implementation**
**Status:** Mentioned but Not Detailed  
**Missing Details:**

```http
# Vendor Creates Invitations
POST   /vendors/me/invitations                              # ✅ Mentioned
GET    /vendors/me/invitations                              # ✅ Mentioned
GET    /vendors/me/invitations/{id}                         # ✅ Mentioned
POST   /vendors/me/invitations/{id}/resend                  # ✅ Mentioned
DELETE /vendors/me/invitations/{id}                         # ✅ Mentioned

# Customer Accepts (Public)
GET    /invitations/{token}                                 # ✅ Mentioned
POST   /invitations/{token}/accept                          # ✅ Mentioned
POST   /invitations/{token}/reject                          # ✅ Mentioned

# ❌ MISSING: Track invitation usage
GET    /vendors/me/invitations/analytics                    # ❌ Invitation stats
GET    /vendors/me/customers/invited                        # ❌ Customers from invites
```

---

### 11. **Coupon System - Missing Features**
**Status:** Basic Features Missing  
**Missing Endpoints:**

```http
GET    /coupons/active                                      # ✅ Exists
POST   /coupons/validate                                    # ✅ Exists
GET    /users/me/coupons/available                          # ❌ Personalized coupons
GET    /users/me/coupons/used                               # ❌ Usage history
POST   /coupons/{code}/claim                                # ❌ Claim coupon

# Vendor-Specific Coupons
POST   /vendors/me/coupons                                  # ❌ Create vendor coupon
GET    /vendors/me/coupons                                  # ❌ List vendor coupons
GET    /vendors/me/coupons/analytics                        # ❌ Coupon performance
```

---

### 12. **Category Management - Missing Features**
**Status:** Basic CRUD Present, Advanced Missing  
**Missing Endpoints:**

```http
GET    /categories/tree                                     # ❌ Hierarchical tree
GET    /categories/{id}/subcategories                       # ❌ Get subcategories
GET    /categories/{id}/breadcrumb                          # ❌ Category path
GET    /categories/popular                                  # ❌ Popular categories
```

---

### 13. **Product Specifications & Variants**
**Status:** Missing Advanced Features  
**Missing Endpoints:**

```http
# Product Variants (Size, Color, etc.)
POST   /vendors/me/products/{id}/variants                   # ❌ Add variant
GET    /vendors/me/products/{id}/variants                   # ❌ List variants
PUT    /vendors/me/products/{id}/variants/{variantId}       # ❌ Update variant
DELETE /vendors/me/products/{id}/variants/{variantId}       # ❌ Delete variant

# Bulk Operations
POST   /vendors/me/products/bulk-update                     # ❌ Bulk update
POST   /vendors/me/products/bulk-import                     # ❌ CSV import
GET    /vendors/me/products/export                          # ❌ Export products
```

---

### 14. **Wishlist - Missing Features**
**Status:** Basic Features Present, Advanced Missing  
**Missing Endpoints:**

```http
GET    /users/me/wishlist                                   # ✅ Exists
POST   /users/me/wishlist                                   # ✅ Exists
DELETE /users/me/wishlist/{productId}                       # ✅ Exists

# ❌ MISSING:
POST   /users/me/wishlist/move-to-cart                      # ❌ Move all to cart
POST   /users/me/wishlist/{productId}/notify-available      # ❌ Notify when in stock
GET    /users/me/wishlist/price-drops                       # ❌ Price drop alerts
```

---

### 15. **Help & Support**
**Status:** Completely Missing  
**Required Endpoints:**

```http
# FAQ
GET    /help/faq                                            # ❌ Frequently asked questions
GET    /help/faq/categories                                 # ❌ FAQ categories

# Support Tickets
POST   /support/tickets                                     # ❌ Create ticket
GET    /users/me/support/tickets                            # ❌ My tickets
GET    /support/tickets/{id}                                # ❌ Ticket details
POST   /support/tickets/{id}/messages                       # ❌ Add message
PATCH  /support/tickets/{id}/close                          # ❌ Close ticket

# Admin Support
GET    /admin/support/tickets                               # ❌ All tickets
PATCH  /admin/support/tickets/{id}/assign                   # ❌ Assign ticket
```

---

### 16. **Settings & Configuration**
**Status:** Missing System Configuration  
**Missing Endpoints:**

```http
# Platform Settings
GET    /settings/app-config                                 # ❌ App configuration
GET    /settings/payment-methods                            # ❌ Available payment methods
GET    /settings/delivery-slots                             # ❌ Available time slots

# User Preferences (Beyond profile)
GET    /users/me/preferences                                # ❌ All preferences
PUT    /users/me/preferences/notifications                  # ❌ Notification settings
PUT    /users/me/preferences/privacy                        # ❌ Privacy settings
```

---

### 17. **Webhooks (For Vendors)**
**Status:** Missing for Vendor Integration  
**Required Endpoints:**

```http
POST   /vendors/me/webhooks                                 # ❌ Register webhook
GET    /vendors/me/webhooks                                 # ❌ List webhooks
DELETE /vendors/me/webhooks/{id}                            # ❌ Delete webhook
POST   /vendors/me/webhooks/{id}/test                       # ❌ Test webhook
GET    /vendors/me/webhooks/{id}/logs                       # ❌ Webhook delivery logs
```

---

## ⚠️ BUSINESS LOGIC GAPS

### 1. **Subscription Settlement Flow**
**Missing:**
- Settlement calculation endpoint
- Settlement dispute handling
- Settlement history

```http
POST   /subscriptions/{id}/settle                           # ✅ Exists
GET    /subscriptions/{id}/settlement-preview               # ❌ Preview settlement
POST   /subscriptions/{id}/settlement/dispute               # ❌ Dispute settlement
GET    /users/me/settlements                                # ❌ Settlement history
```

---

### 2. **Inventory Management**
**Missing:**
- Low stock notifications
- Auto-reorder
- Inventory history

```http
GET    /vendors/me/inventory                                # ❌ Inventory overview
POST   /vendors/me/inventory/adjust                         # ❌ Adjust stock
GET    /vendors/me/inventory/history                        # ❌ Stock movement history
GET    /vendors/me/inventory/low-stock                      # ❌ Low stock alerts
```

---

### 3. **Multi-Location Support**
**Missing:**
- Multiple vendor locations
- Location-based inventory
- Zone management

```http
POST   /vendors/me/locations                                # ❌ Add location
GET    /vendors/me/locations                                # ❌ List locations
PUT    /vendors/me/locations/{id}                           # ❌ Update location
GET    /vendors/me/locations/{id}/inventory                 # ❌ Location inventory
```

---

## 📊 SUMMARY

### Critical Missing (Implement First):
1. ✅ **Notifications System** - Essential for user engagement
2. ✅ **Order Returns & Refunds** - Critical for e-commerce
3. ✅ **Payment Methods Management** - Improve checkout
4. ✅ **Delivery Reschedule** - Customer convenience
5. ✅ **Basic Analytics** - Business insights

### Important Missing (Implement Second):
6. Product search filters & autocomplete
7. Review system enhancements (images, reporting)
8. Subscription modifications (vacation mode, skip dates)
9. Vendor payout details
10. Customer support tickets

### Nice to Have (Future):
11. Advanced analytics & reports
12. Product variants
13. Webhooks for vendors
14. Multi-location support
15. Wallet/Credits system

---

## 📈 UPDATED ENDPOINT COUNT

**Current Planned:** ~120 endpoints  
**After Adding Missing APIs:** ~190 endpoints

### Breakdown:
- **Auth:** 8
- **User Management:** 35 (+8)
- **Vendor Management:** 55 (+17)
- **Catalog:** 30 (+8)
- **Shopping:** 10 (+3)
- **Orders:** 25 (+10)
- **Payments:** 12 (+7)
- **Subscriptions:** 20 (+8)
- **Deliveries:** 15 (+7)
- **Invoices:** 8 (+2)
- **Reviews:** 12 (+4)
- **Notifications:** 7 (NEW)
- **Analytics:** 15 (NEW)
- **Support:** 10 (NEW)
- **Admin:** 20 (+5)

**TOTAL: ~190 Endpoints**

---

## ✅ RECOMMENDATIONS

### Phase 1 (MVP - Week 1-2):
- Implement core missing features (notifications, returns, payment methods)
- Complete subscription management features
- Add basic analytics

### Phase 2 (Enhancement - Week 3-4):
- Advanced search & filters
- Review system enhancements
- Support ticket system
- Vendor analytics

### Phase 3 (Scale - Month 2):
- Product variants
- Multi-location support
- Webhooks
- Advanced analytics
- Wallet system

---

**Last Updated:** February 2026  
**Status:** Comprehensive Analysis Complete
