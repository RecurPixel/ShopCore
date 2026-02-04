using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.Services;

public class PdfService : IPdfService
{
    static PdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> GenerateOrderInvoicePdfAsync(Order order)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeOrderHeader(c, order));
                page.Content().Element(c => ComposeOrderContent(c, order));
                page.Footer().Element(ComposeFooter);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    public Task<byte[]> GenerateDeliveryReceiptAsync(Delivery delivery)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeDeliveryHeader(c, delivery));
                page.Content().Element(c => ComposeDeliveryContent(c, delivery));
                page.Footer().Element(ComposeFooter);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    public Task<byte[]> GenerateSubscriptionInvoicePdfAsync(SubscriptionInvoice invoice)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposeInvoiceHeader(c, invoice));
                page.Content().Element(c => ComposeInvoiceContent(c, invoice));
                page.Footer().Element(ComposeFooter);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    // Order Invoice Components
    private void ComposeOrderHeader(IContainer container, Order order)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("INVOICE").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                col.Item().Text($"Order #{order.OrderNumber}").FontSize(12);
                col.Item().Text($"Date: {order.CreatedAt:dd MMM yyyy}");
            });

            row.ConstantItem(150).Column(col =>
            {
                col.Item().AlignRight().Text("ShopCore").FontSize(16).Bold();
                col.Item().AlignRight().Text("Tax Invoice");
            });
        });

        container.PaddingTop(20);
    }

    private void ComposeOrderContent(IContainer container, Order order)
    {
        container.Column(col =>
        {
            // Customer & Shipping Info
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Bill To:").Bold();
                    c.Item().Text(order.User?.FullName ?? "Customer");
                    c.Item().Text(order.User?.Email ?? "");
                    c.Item().Text(order.User?.PhoneNumber ?? "");
                });

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Ship To:").Bold();
                    if (order.ShippingAddress != null)
                    {
                        c.Item().Text(order.ShippingAddress.FullName);
                        c.Item().Text(order.ShippingAddress.AddressLine1);
                        if (!string.IsNullOrEmpty(order.ShippingAddress.AddressLine2))
                            c.Item().Text(order.ShippingAddress.AddressLine2);
                        c.Item().Text($"{order.ShippingAddress.City}, {order.ShippingAddress.State} - {order.ShippingAddress.Pincode}");
                    }
                });
            });

            col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            // Items Table
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Item").Bold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Qty").Bold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Price").Bold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Total").Bold();
                });

                foreach (var item in order.Items)
                {
                    table.Cell().Padding(5).Text(item.ProductName);
                    table.Cell().Padding(5).AlignCenter().Text(item.Quantity.ToString());
                    table.Cell().Padding(5).AlignRight().Text($"{item.UnitPrice:N2}");
                    table.Cell().Padding(5).AlignRight().Text($"{item.Subtotal:N2}");
                }
            });

            col.Item().PaddingVertical(10);

            // Totals
            col.Item().AlignRight().Width(200).Column(c =>
            {
                c.Item().Row(r =>
                {
                    r.RelativeItem().Text("Subtotal:");
                    r.ConstantItem(80).AlignRight().Text($"{order.Subtotal:N2}");
                });

                if (order.Discount > 0)
                {
                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Discount:");
                        r.ConstantItem(80).AlignRight().Text($"-{order.Discount:N2}");
                    });
                }

                c.Item().Row(r =>
                {
                    r.RelativeItem().Text("Tax (GST 18%):");
                    r.ConstantItem(80).AlignRight().Text($"{order.Tax:N2}");
                });

                if (order.ShippingCharge > 0)
                {
                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Shipping:");
                        r.ConstantItem(80).AlignRight().Text($"{order.ShippingCharge:N2}");
                    });
                }

                c.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Black).Row(r =>
                {
                    r.RelativeItem().Text("Total:").Bold();
                    r.ConstantItem(80).AlignRight().Text($"{order.Total:N2}").Bold();
                });
            });

            // Payment Status
            col.Item().PaddingTop(20).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"Payment Status: {order.PaymentStatus}").Bold();
                    if (order.PaidAt.HasValue)
                        c.Item().Text($"Paid On: {order.PaidAt.Value:dd MMM yyyy HH:mm}");
                    c.Item().Text($"Payment Method: {order.PaymentMethod}");
                });
            });
        });
    }

    // Delivery Receipt Components
    private void ComposeDeliveryHeader(IContainer container, Delivery delivery)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("DELIVERY RECEIPT").FontSize(24).Bold().FontColor(Colors.Green.Darken2);
                col.Item().Text($"#{delivery.DeliveryNumber}").FontSize(12);
                col.Item().Text($"Scheduled: {delivery.ScheduledDate:dd MMM yyyy}");
            });

            row.ConstantItem(150).Column(col =>
            {
                col.Item().AlignRight().Text("ShopCore").FontSize(16).Bold();
                col.Item().AlignRight().Text(delivery.Subscription?.Vendor?.BusinessName ?? "Vendor");
            });
        });

        container.PaddingTop(20);
    }

    private void ComposeDeliveryContent(IContainer container, Delivery delivery)
    {
        container.Column(col =>
        {
            // Customer Info
            col.Item().Column(c =>
            {
                c.Item().Text("Delivered To:").Bold();
                c.Item().Text(delivery.Subscription?.User?.FullName ?? "Customer");
                if (delivery.Subscription?.DeliveryAddress != null)
                {
                    var addr = delivery.Subscription.DeliveryAddress;
                    c.Item().Text(addr.AddressLine1);
                    if (!string.IsNullOrEmpty(addr.AddressLine2))
                        c.Item().Text(addr.AddressLine2);
                    c.Item().Text($"{addr.City}, {addr.State} - {addr.Pincode}");
                }
            });

            col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            // Items Table
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Item").Bold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Qty").Bold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Price").Bold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Amount").Bold();
                });

                foreach (var item in delivery.Items)
                {
                    table.Cell().Padding(5).Text(item.ProductName);
                    table.Cell().Padding(5).AlignCenter().Text(item.Quantity.ToString());
                    table.Cell().Padding(5).AlignRight().Text($"{item.UnitPrice:N2}");
                    table.Cell().Padding(5).AlignRight().Text($"{item.Amount:N2}");
                }
            });

            col.Item().PaddingVertical(10);

            // Total
            col.Item().AlignRight().Width(200).Column(c =>
            {
                c.Item().BorderTop(1).BorderColor(Colors.Black).PaddingTop(5).Row(r =>
                {
                    r.RelativeItem().Text("Total:").Bold();
                    r.ConstantItem(80).AlignRight().Text($"{delivery.TotalAmount:N2}").Bold();
                });
            });

            // Delivery Status
            col.Item().PaddingTop(20).Column(c =>
            {
                c.Item().Text($"Status: {delivery.Status}").Bold();
                if (delivery.ActualDeliveryDate.HasValue)
                    c.Item().Text($"Delivered On: {delivery.ActualDeliveryDate.Value:dd MMM yyyy HH:mm}");
                c.Item().Text($"Payment Status: {delivery.PaymentStatus}");
                if (!string.IsNullOrEmpty(delivery.DeliveryPersonName))
                    c.Item().Text($"Delivered By: {delivery.DeliveryPersonName}");
            });
        });
    }

    // Subscription Invoice Components
    private void ComposeInvoiceHeader(IContainer container, SubscriptionInvoice invoice)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("SUBSCRIPTION INVOICE").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                col.Item().Text($"#{invoice.InvoiceNumber}").FontSize(12);
                col.Item().Text($"Generated: {invoice.GeneratedAt:dd MMM yyyy}");
                col.Item().Text($"Due Date: {invoice.DueDate:dd MMM yyyy}");
            });

            row.ConstantItem(150).Column(col =>
            {
                col.Item().AlignRight().Text(invoice.Vendor?.BusinessName ?? "Vendor").FontSize(16).Bold();
                col.Item().AlignRight().Text("Tax Invoice");
            });
        });

        container.PaddingTop(20);
    }

    private void ComposeInvoiceContent(IContainer container, SubscriptionInvoice invoice)
    {
        container.Column(col =>
        {
            // Customer Info
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Bill To:").Bold();
                    c.Item().Text(invoice.User?.FullName ?? "Customer");
                    c.Item().Text(invoice.User?.Email ?? "");
                    c.Item().Text(invoice.User?.PhoneNumber ?? "");
                });

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Billing Period:").Bold();
                    c.Item().Text($"{invoice.PeriodStart:dd MMM yyyy} - {invoice.PeriodEnd:dd MMM yyyy}");
                    c.Item().Text($"Total Deliveries: {invoice.TotalDeliveries}");
                });
            });

            col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            // Deliveries Table
            if (invoice.Deliveries.Any())
            {
                col.Item().Text("Deliveries Included:").Bold();
                col.Item().PaddingTop(5).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Delivery #").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Date").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Status").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Amount").Bold();
                    });

                    foreach (var delivery in invoice.Deliveries)
                    {
                        table.Cell().Padding(5).Text(delivery.DeliveryNumber);
                        table.Cell().Padding(5).Text($"{delivery.ScheduledDate:dd MMM}");
                        table.Cell().Padding(5).Text(delivery.Status.ToString());
                        table.Cell().Padding(5).AlignRight().Text($"{delivery.TotalAmount:N2}");
                    }
                });
            }

            col.Item().PaddingVertical(10);

            // Totals
            col.Item().AlignRight().Width(200).Column(c =>
            {
                c.Item().Row(r =>
                {
                    r.RelativeItem().Text("Subtotal:");
                    r.ConstantItem(80).AlignRight().Text($"{invoice.Subtotal:N2}");
                });

                c.Item().Row(r =>
                {
                    r.RelativeItem().Text("Tax (GST 18%):");
                    r.ConstantItem(80).AlignRight().Text($"{invoice.Tax:N2}");
                });

                c.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Black).Row(r =>
                {
                    r.RelativeItem().Text("Total:").Bold();
                    r.ConstantItem(80).AlignRight().Text($"{invoice.Total:N2}").Bold();
                });

                if (invoice.PaidAmount > 0)
                {
                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Paid:");
                        r.ConstantItem(80).AlignRight().Text($"-{invoice.PaidAmount:N2}");
                    });

                    c.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Balance Due:").Bold();
                        r.ConstantItem(80).AlignRight().Text($"{invoice.BalanceDue:N2}").Bold();
                    });
                }
            });

            // Payment Status
            col.Item().PaddingTop(20).Column(c =>
            {
                c.Item().Text($"Status: {invoice.Status}").Bold();
                if (invoice.PaidAt.HasValue)
                {
                    c.Item().Text($"Paid On: {invoice.PaidAt.Value:dd MMM yyyy HH:mm}");
                    c.Item().Text($"Payment Method: {invoice.PaymentMethod}");
                }
                if (invoice.IsOverdue)
                    c.Item().Text("OVERDUE").FontColor(Colors.Red.Medium).Bold();
            });
        });
    }

    // Footer
    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Generated by ShopCore | ");
            text.Span($"{DateTime.UtcNow:dd MMM yyyy HH:mm} UTC");
        });
    }
}
