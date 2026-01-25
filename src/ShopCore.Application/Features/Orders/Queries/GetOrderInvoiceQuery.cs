namespace ShopCore.Application.Orders.Queries.GetOrderInvoice;

public record GetOrderInvoiceQuery(int Id) : IRequest<object>;
