using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.Application.Commands.ChangeUserRole;
using UsersAPI.Application.Commands.DeleteUser;
using UsersAPI.Application.Commands.NewLogin;
using UsersAPI.Application.Commands.RegisterUser;
using UsersAPI.Application.Commands.UpdateUser;
using UsersAPI.Application.Queries.GetUserByEmail;
using UsersAPI.Application.Queries.GetUserById;
using UsersAPI.Application.Queries.GetUserByName;
using UsersAPI.Application.Queries.GetUsers;

namespace UsersAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class 
    UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>Registers a new user</summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    /// <summary>Authenticates a user and returns a JWT token</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] NewLoginCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    /// <summary>Lists users (Admin only)</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetUsersQuery query, CancellationToken ct)
    {
        var result = await mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    /// <summary>Gets user by ID (Admin only)</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Message);
    }

    /// <summary>Gets user by email (Admin only)</summary>
    [HttpGet("email")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail([FromQuery] string email, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByEmailQuery(email), ct);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Message);
    }

    /// <summary>Gets users by name (Admin only)</summary>
    [HttpGet("name")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName([FromQuery] string name, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserByNameQuery(name), ct);
        return result.IsSuccess ? Ok(result.Data) : NotFound(result.Message);
    }

    /// <summary>Updates a user (Admin only)</summary>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest body, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateUserCommand(id, body.Name, body.Email, body.IsActive), ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    /// <summary>Changes a user's role (Admin only)</summary>
    [HttpPatch("{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeUserRoleRequest body, CancellationToken ct)
    {
        var result = await mediator.Send(new ChangeUserRoleCommand(id, body.Role), ct);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    /// <summary>Deletes a user (Admin only)</summary>
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
