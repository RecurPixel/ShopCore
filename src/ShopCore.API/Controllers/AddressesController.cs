using ShopCore.Application.Addresses.Commands.CreateAddress;
using ShopCore.Application.Addresses.Commands.DeleteAddress;
using ShopCore.Application.Addresses.Commands.SetDefaultAddress;
using ShopCore.Application.Addresses.Commands.UpdateAddress;
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
    public async Task<IActionResult> GetMyAddresses()
    {
        var addresses = await _mediator.Send(new GetMyAddressesQuery());
        return Ok(addresses);
    }

    // POST /api/v1/users/me/addresses
    [HttpPost]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressCommand command)
    {
        var address = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetMyAddresses), new { }, address);
    }

    // PUT /api/v1/users/me/addresses/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressCommand command)
    {
        command.AddressId = id;

        var address = await _mediator.Send(command);
        return Ok(address);
    }

    // DELETE /api/v1/users/me/addresses/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        await _mediator.Send(new DeleteAddressCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/users/me/addresses/{id}/default
    [HttpPatch("{id}/default")]
    public async Task<IActionResult> SetDefaultAddress(Guid id)
    {
        await _mediator.Send(new SetDefaultAddressCommand(id));
        return NoContent();
    }
}
