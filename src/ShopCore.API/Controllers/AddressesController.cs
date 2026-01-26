using ShopCore.Application.Addresses.Commands.CreateAddress;
using ShopCore.Application.Addresses.Commands.DeleteAddress;
using ShopCore.Application.Addresses.Commands.SetDefaultAddress;
using ShopCore.Application.Addresses.Commands.UpdateAddress;
using ShopCore.Application.Addresses.DTOs;
using ShopCore.Application.Addresses.Queries.GetAddressById;
using ShopCore.Application.Addresses.Queries.GetMyAddresses;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AddressesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AddressesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/v1/users/me/addresses
    [HttpGet]
    public async Task<ActionResult<List<AddressDto>>> GetMyAddresses()
    {
        var addresses = await _mediator.Send(new GetMyAddressesQuery());
        return Ok(addresses);
    }

    // GET /api/v1/users/me/addresses/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AddressDto>> GetAddressById(int id)
    {
        var address = await _mediator.Send(
            new GetAddressByIdQuery(id));

        if (address is null)
            return NotFound();

        return Ok(address);
    }

    // POST /api/v1/users/me/addresses
    [HttpPost]
    public async Task<ActionResult<AddressDto>> CreateAddress(
        [FromBody] CreateAddressCommand command)
    {
        var address = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetAddressById),
            new { id = address.Id },
            address);
    }

    // PUT /api/v1/users/me/addresses/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<AddressDto>> UpdateAddress(
        int id,
        [FromBody] UpdateAddressCommand command)
    {
        // Enforce route → command consistency
        var updatedCommand = command with { Id = id };

        var address = await _mediator.Send(updatedCommand);
        return Ok(address);
    }

    // DELETE /api/v1/users/me/addresses/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        await _mediator.Send(new DeleteAddressCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/users/me/addresses/{id}/default
    [HttpPatch("{id:int}/default")]
    public async Task<IActionResult> SetDefaultAddress(int id)
    {
        await _mediator.Send(new SetDefaultAddressCommand(id));
        return NoContent();
    }
}
