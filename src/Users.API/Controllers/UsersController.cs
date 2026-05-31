using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Commands.ChangeUserRole;
using Users.Application.Commands.DeleteUser;
using Users.Application.Commands.NewLogin;
using Users.Application.Commands.RegisterUser;
using Users.Application.Commands.UpdateUser;
using Users.Application.Queries.GetUserByEmail;
using Users.Application.Queries.GetUserById;
using Users.Application.Queries.GetUserByName;
using Users.Application.Queries.GetUsers;

namespace Users.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] NewLoginCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetUsersQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Message);
    }

    [HttpGet("email")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail([FromQuery] string email, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByEmailQuery(email), ct);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Message);
    }

    [HttpGet("name")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName([FromQuery] string name, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByNameQuery(name), ct);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Message);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateUserCommand(id, body.Name, body.Email, body.IsActive), ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpPatch("{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeUserRoleRequest body, CancellationToken ct)
    {
        var result = await mediator.Send(new ChangeUserRoleCommand(id, body.Role), ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new DeleteUserCommand(id), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Message);
    }
}

public record UpdateUserRequest(string? Name = null, string? Email = null, bool? IsActive = null);
public record ChangeUserRoleRequest(string Role);
