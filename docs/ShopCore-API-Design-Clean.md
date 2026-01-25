# ShopCore API Design

**Version:** 1.0  
**Base URL:** `/api/v1`  
**Authentication:** JWT Bearer Token  
**Response Standard:** RFC 7807 (ProblemDetails) + Direct returns

---

## Response Patterns

### Success Responses

**Single Resource:**
```http
GET /api/v1/products/123
200 OK

{
  "id": 123,
  "name": "Full Cream Milk 1L",
  "price": 60.00,
  "categoryId": 5,
  "vendorId": 10
}
```

**List (No Pagination):**
```http
GET /api/v1/categories
200 OK

[
  { "id": 1, "name": "Dairy" },
  { "id": 2, "name": "Beverages" }
]
```

**List (With Pagination):**
```http
GET /api/v1/products?page=1&pageSize=20
200 OK

{
  "items": [
    { "id": 1, "name": "Product 1" },
    { "id": 2, "name": "Product 2" }
  ],
  "page": 1,
  "pageSize": 20,
  "totalPages": 5,
  "totalItems": 98,
  "hasNext": true,
  "hasPrevious": false
}
```

**Create:**
```http
POST /api/v1/products
201 Created
Location: /api/v1/products/123

{
  "id": 123,
  "name": "New Product",
  "price": 100.00
}
```

**Update:**
```http
PUT /api/v1/products/123
204 No Content
```

**Delete:**
```http
DELETE /api/v1/products/123
204 No Content
```

---

### Error Responses (RFC 7807)

**Validation Error (400):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["The Email field is required."],
    "Password": ["Password must be at least 8 characters."]
  },
  "traceId": "00-abc123-def456-00"
}
```

**Not Found (404):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Product with ID 999 was not found.",
  "traceId": "00-xyz789-abc123-00"
}
```

**Unauthorized (401):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Invalid credentials.",
  "traceId": "00-token123-auth456-00"
}
```

**Forbidden (403):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "You do not have permission to access this resource.",
  "traceId": "00-perm789-deny012-00"
}
```

**Server Error (500):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "traceId": "00-error123-server456-00"
}
```

---

## Endpoints

### Authentication (`/api/v1/auth`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/register` | - | Register new user |
| POST | `/login` | - | Login and get JWT token |
| POST | `/refresh-token` | - | Refresh access token |
| POST | `/logout` | ✓ | Logout (invalidate refresh token) |
| POST | `/forgot-password` | - | Request password reset |
| POST | `/reset-password` | - | Reset password with token |
| POST | `/verify-email` | - | Verify email with token |

**Register:**
```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "9876543210",
  "role": "Customer"
}

Response: 201 Created
{
  "id": 123,
  "email": "user@example.com",
  "firstName": "John",
  "role": "Customer",
  "emailVerified": false
}
```

**Login:**
```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}

Response: 200 OK
{
  "accessToken": "eyJhbGci...",
  "refreshToken": "abc123...",
  "expiresIn": 3600,
  "user": {
    "id": 123,
    "email": "user@example.com",
    "role": "Customer"
  }
}
```

---

### Users (`/api/v1/users`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/me` | ✓ | Get current user profile |
| PUT | `/me` | ✓ | Update profile |
| POST | `/me/avatar` | ✓ | Upload avatar |
| POST | `/me/change-password` | ✓ | Change password |

---

### Addresses (`/api/v1/users/me/addresses`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/` | ✓ | List user addresses |
| POST | `/` | ✓ | Add new address |
| PUT | `/{id}` | ✓ | Update address |
| DELETE | `/{id}` | ✓ | Delete address |
| PATCH | `/{id}/default` | ✓ | Set as default |

---

### Vendors (`/api/v1/vendors`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/register` | ✓ | Register as vendor |
| GET | `/me` | ✓ | Get vendor profile |
| PUT | `/me` | ✓ | Update profile |
| GET | `/me/stats` | ✓ | Get statistics |
| GET | `/me/orders` | ✓ | List vendor orders |
| PATCH | `/me/orders/{id}/status` | ✓ | Update order status |

**Admin:**
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/pending` | Admin | List pending vendors |
| PATCH | `/{id}/approve` | Admin | Approve vendor |
| PATCH | `/{id}/suspend` | Admin | Suspend vendor |

---

### Categories (`/api/v1/categories`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/` | - | List all categories |
| GET | `/{id}` | - | Get category details |
| GET | `/{id}/products` | - | Products in category |
| POST | `/` | Admin | Create category |
| PUT | `/{id}` | Admin | Update category |
| DELETE | `/{id}` | Admin | Delete category |

---

### Products (`/api/v1/products`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/` | - | List products (paginated) |
| GET | `/search` | - | Search products |
| GET | `/featured` | - | Featured products |
| GET | `/{id}` | - | Get product details |
| POST | `/` | Vendor | Create product |
| PUT | `/{id}` | Vendor | Update product |
| DELETE | `/{id}` | Vendor | Delete product |
| POST | `/{id}/images` | Vendor | Upload images |
| DELETE | `/{id}/images/{imageId}` | Vendor | Delete image |
| PATCH | `/{id}/status` | Vendor | Update status |

**Query Parameters:**
```
page=1
pageSize=20
categoryId=5
vendorId=10
minPrice=0
maxPrice=1000
inStock=true
sortBy=price-asc|price-desc|rating|newest
```

---

### Cart (`/api/v1/cart`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/` | ✓ | Get cart |
| POST | `/items` | ✓ | Add item |
| PUT | `/items/{id}` | ✓ | Update quantity |
| DELETE | `/items/{id}` | ✓ | Remove item |
| DELETE | `/clear` | ✓ | Clear cart |
| POST | `/validate` | ✓ | Validate cart |

**Cart Response:**
```json
{
  "id": 123,
  "userId": 456,
  "items": [
    {
      "id": 1,
      "productId": 10,
      "productName": "Milk 1L",
      "price": 60.00,
      "quantity": 2,
      "subtotal": 120.00,
      "vendorId": 5
    }
  ],
  "subtotal": 120.00,
  "tax": 21.60,
  "total": 141.60,
  "itemCount": 2
}
```

---

### Orders (`/api/v1/orders`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/` | ✓ | Create order (checkout) |
| GET | `/` | ✓ | List user orders |
| GET | `/{id}` | ✓ | Get order details |
| POST | `/{id}/cancel` | ✓ | Cancel order |
| GET | `/{id}/invoice` | ✓ | Download invoice PDF |

**Create Order:**
```http
POST /api/v1/orders
Content-Type: application/json
Authorization: Bearer {token}

{
  "addressId": 10,
  "paymentMethod": "Online",
  "couponCode": "WELCOME10"
}

Response: 201 Created
{
  "id": 789,
  "orderNumber": "ORD-2025-0123-789",
  "total": 141.60,
  "status": "Pending",
  "paymentStatus": "Unpaid",
  "items": [...],
  "createdAt": "2025-01-24T10:30:00Z"
}
```

---

### Payments (`/api/v1/payments`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/create-intent` | ✓ | Create payment intent |
| POST | `/confirm` | ✓ | Confirm payment |
| POST | `/webhook` | - | Payment webhook |
| GET | `/history` | ✓ | Payment history |

---

### Reviews (`/api/v1/reviews`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/products/{id}/reviews` | - | Product reviews |
| POST | `/` | ✓ | Create review |
| PUT | `/{id}` | ✓ | Update review |
| DELETE | `/{id}` | ✓ | Delete review |
| POST | `/{id}/helpful` | ✓ | Mark helpful |
| POST | `/{id}/respond` | Vendor | Vendor response |

---

### Wishlist (`/api/v1/wishlist`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/` | ✓ | Get wishlist |
| POST | `/` | ✓ | Add product |
| DELETE | `/{productId}` | ✓ | Remove product |

---

### Coupons (`/api/v1/coupons`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/active` | ✓ | List active coupons |
| POST | `/validate` | ✓ | Validate coupon code |

**Admin:**
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/` | Admin | List all coupons |
| POST | `/` | Admin | Create coupon |
| PATCH | `/{id}/deactivate` | Admin | Deactivate coupon |

---

## Subscriptions (PRIVATE)

### Subscriptions (`/api/v1/subscriptions`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/` | ✓ | Create subscription |
| GET | `/` | ✓ | List subscriptions |
| GET | `/{id}` | ✓ | Get details |
| PATCH | `/{id}` | ✓ | Update settings |
| PATCH | `/{id}/pause` | ✓ | Pause subscription |
| PATCH | `/{id}/resume` | ✓ | Resume subscription |
| POST | `/{id}/settle` | ✓ | Settle and cancel |

**Create Subscription:**
```http
POST /api/v1/subscriptions
Content-Type: application/json
Authorization: Bearer {token}

{
  "productId": 10,
  "quantity": 1,
  "frequency": "Daily",
  "startDate": "2025-02-01",
  "addressId": 5,
  "preferredDeliveryTime": "08:00",
  "depositAmount": 500,
  "billingCycleDays": 30
}

Response: 201 Created
{
  "id": 123,
  "productId": 10,
  "status": "Active",
  "nextDeliveryDate": "2025-02-01",
  "nextBillingDate": "2025-03-01",
  "unpaidAmount": 0.00,
  "depositBalance": 500.00
}
```

---

### Deliveries (`/api/v1/deliveries`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/subscriptions/{id}/deliveries` | ✓ | List deliveries |
| GET | `/{id}` | ✓ | Get delivery details |
| POST | `/{id}/skip` | ✓ | Skip delivery |
| PATCH | `/{id}/complete` | Vendor | Mark delivered |
| PATCH | `/{id}/failed` | Vendor | Mark failed |

---

### Invoices (`/api/v1/invoices`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/subscriptions/{id}/invoices` | ✓ | List invoices |
| GET | `/{id}` | ✓ | Get invoice |
| POST | `/{id}/pay` | ✓ | Pay invoice |
| GET | `/{id}/download` | ✓ | Download PDF |
| POST | `/subscriptions/{id}/generate` | Vendor | Generate manually |

---

## CQRS Pattern

### Commands (Write Operations)
```csharp
// Command
public record CreateProductCommand(
    string Name,
    decimal Price,
    int CategoryId
) : IRequest<Product>;

// Handler
public class CreateProductCommandHandler 
    : IRequestHandler<CreateProductCommand, Product>
{
    public async Task<Product> Handle(
        CreateProductCommand request, 
        CancellationToken cancellationToken)
    {
        // Business logic
        // Save to database
        return product;
    }
}

// Controller
[HttpPost]
public async Task<IActionResult> CreateProduct(
    [FromBody] CreateProductCommand command)
{
    var product = await _mediator.Send(command);
    return CreatedAtAction(
        nameof(GetProduct), 
        new { id = product.Id }, 
        product
    );
}
```

### Queries (Read Operations)
```csharp
// Query
public record GetProductByIdQuery(int Id) : IRequest<Product>;

// Handler
public class GetProductByIdQueryHandler 
    : IRequestHandler<GetProductByIdQuery, Product>
{
    public async Task<Product> Handle(
        GetProductByIdQuery request, 
        CancellationToken cancellationToken)
    {
        return await _context.Products.FindAsync(request.Id);
    }
}

// Controller
[HttpGet("{id}")]
public async Task<IActionResult> GetProduct(int id)
{
    var product = await _mediator.Send(new GetProductByIdQuery(id));
    
    if (product == null)
        return NotFound(new ProblemDetails 
        { 
            Status = 404,
            Title = "Not Found",
            Detail = $"Product with ID {id} was not found."
        });
    
    return Ok(product);
}
```

---

## Common Headers

**Request:**
```
Authorization: Bearer {jwt_token}
Content-Type: application/json
Accept: application/json
```

**Response:**
```
Content-Type: application/json
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1642857600
```

---

## Status Codes

| Code | Description |
|------|-------------|
| 200 | OK - Request successful |
| 201 | Created - Resource created |
| 204 | No Content - Update/delete successful |
| 400 | Bad Request - Validation error |
| 401 | Unauthorized - Authentication required |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found - Resource doesn't exist |
| 409 | Conflict - Resource already exists |
| 422 | Unprocessable Entity - Business rule violation |
| 429 | Too Many Requests - Rate limit exceeded |
| 500 | Internal Server Error - Server error |

---

**Total Endpoints: ~85**
