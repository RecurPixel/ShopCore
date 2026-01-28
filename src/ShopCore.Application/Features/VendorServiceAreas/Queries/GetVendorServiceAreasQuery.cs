using ShopCore.Application.VendorServiceAreas.DTOs;

namespace ShopCore.Application.VendorServiceAreas.Queries.GetVendorServiceAreas;

public record GetVendorServiceAreasQuery(int? VendorId = null) : IRequest<List<VendorServiceAreaDto>>;
