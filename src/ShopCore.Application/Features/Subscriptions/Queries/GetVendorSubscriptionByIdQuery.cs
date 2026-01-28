using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptionById;

public record GetVendorSubscriptionByIdQuery(int Id) : IRequest<VendorSubscriptionDto?>;
