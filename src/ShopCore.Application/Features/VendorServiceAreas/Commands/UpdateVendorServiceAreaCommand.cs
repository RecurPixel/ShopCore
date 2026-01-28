using ShopCore.Application.VendorServiceAreas.DTOs;

namespace ShopCore.Application.VendorServiceAreas.Commands.UpdateVendorServiceArea;

public record UpdateVendorServiceAreaCommand(
    int ServiceAreaId,
    string AreaName,
    List<string> Pincodes,
    bool IsActive
) : IRequest<VendorServiceAreaDto>;
