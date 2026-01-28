using ShopCore.Application.VendorServiceAreas.DTOs;

namespace ShopCore.Application.VendorServiceAreas.Commands.AddVendorServiceArea;

public record AddVendorServiceAreaCommand(
    string AreaName,
    string City,
    string State,
    List<string> Pincodes
) : IRequest<VendorServiceAreaDto>;
