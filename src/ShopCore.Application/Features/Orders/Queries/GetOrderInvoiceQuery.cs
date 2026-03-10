namespace ShopCore.Application.Orders.Queries.GetOrderInvoice;

public record GetOrderInvoiceQuery(int OrderId) : IRequest<FileResponse>;
