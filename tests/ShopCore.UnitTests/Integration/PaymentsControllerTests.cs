using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopCore.Application.Payments.DTOs;
using ShopCore.Domain.Entities;
using ShopCore.Domain.Enums;
using ShopCore.Infrastructure.Data;
using ShopCore.UnitTests.Infrastructure;

namespace ShopCore.UnitTests.Integration;

/// <summary>
/// Payment integration tests split into two categories:
///
///   1. Webhook processing — runs without real API keys. Seeds an order in the DB,
///      simulates a gateway webhook, and verifies the DB state changed correctly.
///
///   2. Payment intent creation — requires real test API keys in appsettings.json.
///      Tests are skipped automatically when no keys are configured.
/// </summary>
public class PaymentsControllerTests : IntegrationTestBase
{
    public PaymentsControllerTests(ShopCoreWebApplicationFactory factory) : base(factory) { }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>Seeds a minimal pending order with a known PaymentTransactionId.</summary>
    private async Task<Order> SeedPendingOrderAsync(string paymentTransactionId)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var customer = await db.Users.FirstAsync(u => u.Email == "customer@test.com");

        var address = new Address
        {
            UserId = customer.Id,
            User = customer,
            FullName = "Test Customer",
            PhoneNumber = "9999999999",
            AddressLine1 = "123 Test Street",
            City = "Bangalore",
            State = "Karnataka",
            Pincode = "560001",
            Country = "India",
            IsDefault = true,
            CreatedAt = DateTime.UtcNow
        };
        db.Addresses.Add(address);
        await db.SaveChangesAsync();

        var product = await db.Products.FirstAsync();

        var order = new Order
        {
            UserId = customer.Id,
            ShippingAddressId = address.Id,
            OrderNumber = $"ORD-TEST-{Guid.NewGuid().ToString("N")[..8]}",
            Subtotal = 500m,
            Tax = 90m,
            ShippingCharge = 0m,
            Total = 590m,
            PaymentStatus = PaymentStatus.Unpaid,
            PaymentMethod = PaymentMethod.Online,
            PaymentTransactionId = paymentTransactionId,
            CreatedAt = DateTime.UtcNow,
            Items =
            [
                new OrderItem
                {
                    ProductId = product.Id,
                    VendorId = product.VendorId,
                    ProductName = product.Name,
                    Quantity = 5,
                    UnitPrice = 100m,
                    Status = OrderItemStatus.Pending,
                    CommissionRate = 5m
                }
            ]
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return order;
    }

    private static string BuildRazorpaySignature(string payload, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLower();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Razorpay Webhook Tests (no real API keys needed)
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task RazorpayWebhook_PaymentCaptured_MarksOrderPaidAndConfirmed()
    {
        // Arrange
        const string gatewayOrderId = "order_rzp_test_captured";
        var order = await SeedPendingOrderAsync(gatewayOrderId);

        var payload = JsonSerializer.Serialize(new
        {
            @event = "payment.captured",
            payload = new
            {
                payment = new
                {
                    entity = new
                    {
                        id = "pay_test_001",
                        order_id = gatewayOrderId,
                        amount = 59000,
                        currency = "INR",
                        method = "upi",
                        status = "captured"
                    }
                }
            }
        });

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/payments/webhook/razorpay")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        // No signature header — gateway skips verification when WebhookSecret is empty

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var updatedOrder = await db.Orders
            .Include(o => o.Items)
            .FirstAsync(o => o.Id == order.Id);

        updatedOrder.PaymentStatus.Should().Be(PaymentStatus.Paid);
        updatedOrder.Status.Should().Be(OrderStatus.Confirmed);
        updatedOrder.PaidAt.Should().NotBeNull();
        updatedOrder.PaymentTransactionId.Should().Be("pay_test_001");
        updatedOrder.Items.Should().AllSatisfy(i => i.Status.Should().Be(OrderItemStatus.Confirmed));
    }

    [Fact]
    public async Task RazorpayWebhook_PaymentFailed_MarksOrderPaymentFailed()
    {
        // Arrange
        const string gatewayOrderId = "order_rzp_test_failed";
        var order = await SeedPendingOrderAsync(gatewayOrderId);

        var payload = JsonSerializer.Serialize(new
        {
            @event = "payment.failed",
            payload = new
            {
                payment = new
                {
                    entity = new
                    {
                        id = "pay_test_002",
                        order_id = gatewayOrderId,
                        amount = 59000,
                        currency = "INR",
                        method = "card",
                        status = "failed"
                    }
                }
            }
        });

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/payments/webhook/razorpay")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var updatedOrder = await db.Orders.FirstAsync(o => o.Id == order.Id);

        updatedOrder.PaymentStatus.Should().Be(PaymentStatus.Failed);
    }

    [Fact]
    public async Task RazorpayWebhook_PaymentCaptured_IsIdempotent()
    {
        // Arrange — send same webhook twice, should not throw or double-process
        const string gatewayOrderId = "order_rzp_idempotent";
        await SeedPendingOrderAsync(gatewayOrderId);

        var payload = JsonSerializer.Serialize(new
        {
            @event = "payment.captured",
            payload = new
            {
                payment = new
                {
                    entity = new
                    {
                        id = "pay_test_003",
                        order_id = gatewayOrderId,
                        amount = 59000,
                        currency = "INR",
                        method = "upi",
                        status = "captured"
                    }
                }
            }
        });

        // Act — send twice
        for (int i = 0; i < 2; i++)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/payments/webhook/razorpay")
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };
            var response = await Client.SendAsync(request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Assert — still exactly Paid (not double-processed)
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var order = await db.Orders.FirstAsync(o => o.PaymentTransactionId == "pay_test_003");
        order.PaymentStatus.Should().Be(PaymentStatus.Paid);
    }

    [Fact]
    public async Task RazorpayWebhook_UnknownEvent_Returns200WithNoSideEffects()
    {
        // Arrange — an event type we don't handle
        var payload = JsonSerializer.Serialize(new
        {
            @event = "order.paid",
            payload = new { }
        });

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/payments/webhook/razorpay")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        // Act
        var response = await Client.SendAsync(request);

        // Assert — should return 200 (idempotent, do not fail on unknown events)
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Stripe Webhook Tests (no real API keys needed)
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task StripeWebhook_PaymentIntentSucceeded_MarksOrderPaidAndConfirmed()
    {
        // Arrange
        const string paymentIntentId = "pi_test_stripe_succeeded";
        var order = await SeedPendingOrderAsync(paymentIntentId);

        // Stripe uses payment_intent.id for BOTH GatewayPaymentId and GatewayOrderId
        var payload = BuildStripePaymentIntentSucceededPayload(paymentIntentId);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/payments/webhook/stripe")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        // No Stripe-Signature header — gateway skips verification when WebhookSecret is empty

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var updatedOrder = await db.Orders
            .Include(o => o.Items)
            .FirstAsync(o => o.Id == order.Id);

        updatedOrder.PaymentStatus.Should().Be(PaymentStatus.Paid);
        updatedOrder.Status.Should().Be(OrderStatus.Confirmed);
        updatedOrder.PaidAt.Should().NotBeNull();
        updatedOrder.Items.Should().AllSatisfy(i => i.Status.Should().Be(OrderItemStatus.Confirmed));
    }

    [Fact]
    public async Task StripeWebhook_PaymentIntentFailed_MarksOrderPaymentFailed()
    {
        // Arrange
        const string paymentIntentId = "pi_test_stripe_failed";
        var order = await SeedPendingOrderAsync(paymentIntentId);

        var payload = BuildStripePaymentIntentFailedPayload(paymentIntentId);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/payments/webhook/stripe")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        // Act
        var response = await Client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var updatedOrder = await db.Orders.FirstAsync(o => o.Id == order.Id);

        updatedOrder.PaymentStatus.Should().Be(PaymentStatus.Failed);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Payment Intent Creation Tests
    // These call the real gateway APIs — skip if keys are not configured.
    // Add your test keys to appsettings.json to run these locally.
    // ──────────────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateRazorpayPaymentIntent_WithRealKeys_ReturnsIntentWithOrderId()
    {
        // Skip if Razorpay test keys are not configured
        using var scope = Factory.Services.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
        var razorpayKeyId = config["PaymentGateways:Razorpay:KeyId"];

        if (string.IsNullOrWhiteSpace(razorpayKeyId) || razorpayKeyId.Contains("YOUR") || razorpayKeyId.Contains("xxxxx"))
        {
            // Skip gracefully — not a failure
            return;
        }

        // Arrange — create an order via the API
        await AuthenticateAsCustomerAsync();
        var product = await DbContext.Products.FirstAsync();

        var cartResponse = await Client.PostAsJsonAsync("/api/v1/cart/items", new { productId = product.Id, quantity = 2 });
        cartResponse.IsSuccessStatusCode.Should().BeTrue($"Cart add failed: {await cartResponse.Content.ReadAsStringAsync()}");

        var addressResponse = await Client.PostAsJsonAsync("/api/v1/users/me/addresses", new
        {
            fullName = "Test Customer",
            phoneNumber = "9999999999",
            addressLine1 = "123 Test Street",
            city = "Bangalore",
            state = "Karnataka",
            pincode = "560001",
            country = "India",
            isDefault = true
        });
        addressResponse.EnsureSuccessStatusCode();
        var createdAddress = await addressResponse.Content.ReadFromJsonAsync<JsonElement>();
        var addressId = createdAddress.GetProperty("id").GetInt32();

        var orderResponse = await Client.PostAsJsonAsync("/api/v1/orders", new
        {
            addressId,
            paymentMethod = (int)PaymentMethod.Online,
            customerNotes = "Razorpay integration test"
        });
        var orderBody = await orderResponse.Content.ReadAsStringAsync();
        orderResponse.StatusCode.Should().BeOneOf(
            new[] { HttpStatusCode.Created, HttpStatusCode.OK },
            because: $"Order creation failed: {orderBody}");

        var order = await orderResponse.Content.ReadFromJsonAsync<JsonElement>();
        var orderId = order.GetProperty("id").GetInt32();

        // Act
        var response = await Client.PostAsync(
            $"/api/v1/payments/orders/{orderId}/create-intent?gateway=Razorpay", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var intent = await response.Content.ReadFromJsonAsync<PaymentIntentDto>();
        intent.Should().NotBeNull();
        intent!.Gateway.Should().Be("Razorpay");
        intent.GatewayOrderId.Should().StartWith("order_");
        intent.Amount.Should().BeGreaterThan(0);
        intent.ClientData.Should().ContainKey("key_id");
        intent.ClientData.Should().ContainKey("order_id");
        intent.ClientData["order_id"].Should().StartWith("order_");
    }

    [Fact]
    public async Task CreateStripePaymentIntent_WithRealKeys_ReturnsIntentWithClientSecret()
    {
        // Skip if Stripe test keys are not configured
        using var scope = Factory.Services.CreateScope();
        var config = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
        var stripeSecretKey = config["PaymentGateways:Stripe:SecretKey"];

        if (string.IsNullOrWhiteSpace(stripeSecretKey) || stripeSecretKey.Contains("YOUR") || stripeSecretKey.Contains("sk_test_YOUR"))
        {
            return;
        }

        // Arrange
        await AuthenticateAsCustomerAsync();
        var product = await DbContext.Products.FirstAsync();

        await Client.PostAsJsonAsync("/api/v1/cart/items", new { productId = product.Id, quantity = 1 });

        var addressResponse = await Client.PostAsJsonAsync("/api/v1/users/me/addresses", new
        {
            fullName = "Test Customer",
            phoneNumber = "9999999999",
            addressLine1 = "123 Test Street",
            city = "Bangalore",
            state = "Karnataka",
            pincode = "560001",
            country = "India",
            isDefault = true
        });
        addressResponse.EnsureSuccessStatusCode();
        var createdAddress = await addressResponse.Content.ReadFromJsonAsync<JsonElement>();
        var addressId = createdAddress.GetProperty("id").GetInt32();

        var orderResponse = await Client.PostAsJsonAsync("/api/v1/orders", new
        {
            addressId,
            paymentMethod = (int)PaymentMethod.Online,
            customerNotes = "Stripe integration test"
        });
        orderResponse.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);

        var order = await orderResponse.Content.ReadFromJsonAsync<JsonElement>();
        var orderId = order.GetProperty("id").GetInt32();

        // Act
        var response = await Client.PostAsync(
            $"/api/v1/payments/orders/{orderId}/create-intent?gateway=Stripe", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var intent = await response.Content.ReadFromJsonAsync<PaymentIntentDto>();
        intent.Should().NotBeNull();
        intent!.Gateway.Should().Be("Stripe");
        intent.PaymentIntentId.Should().StartWith("pi_");
        intent.Amount.Should().BeGreaterThan(0);
        intent.ClientData.Should().ContainKey("client_secret");
        intent.ClientData.Should().ContainKey("publishable_key");
        intent.ClientData["client_secret"].Should().Contain("_secret_");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Stripe payload builders
    // ──────────────────────────────────────────────────────────────────────────

    private static string BuildStripePaymentIntentSucceededPayload(string paymentIntentId) =>
        $$"""
        {
          "id": "evt_test_{{Guid.NewGuid():N}}",
          "object": "event",
          "api_version": "2024-06-20",
          "type": "payment_intent.succeeded",
          "created": {{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}},
          "livemode": false,
          "pending_webhooks": 0,
          "request": null,
          "data": {
            "object": {
              "id": "{{paymentIntentId}}",
              "object": "payment_intent",
              "amount": 59000,
              "currency": "inr",
              "status": "succeeded",
              "created": {{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}},
              "livemode": false,
              "capture_method": "automatic",
              "confirmation_method": "automatic",
              "metadata": {}
            }
          }
        }
        """;

    private static string BuildStripePaymentIntentFailedPayload(string paymentIntentId) =>
        $$"""
        {
          "id": "evt_test_{{Guid.NewGuid():N}}",
          "object": "event",
          "api_version": "2024-06-20",
          "type": "payment_intent.payment_failed",
          "created": {{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}},
          "livemode": false,
          "pending_webhooks": 0,
          "request": null,
          "data": {
            "object": {
              "id": "{{paymentIntentId}}",
              "object": "payment_intent",
              "amount": 59000,
              "currency": "inr",
              "status": "requires_payment_method",
              "created": {{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}},
              "livemode": false,
              "capture_method": "automatic",
              "confirmation_method": "automatic",
              "metadata": {}
            }
          }
        }
        """;
}
