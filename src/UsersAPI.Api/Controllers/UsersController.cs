using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;
using UsersAPI.Application.Commands.ChangeUserRole;
using UsersAPI.Application.Commands.DeleteUser;
using UsersAPI.Application.Commands.NewLogin;
using UsersAPI.Application.Commands.RegisterUser;
using UsersAPI.Application.Commands.UpdateUser;
using UsersAPI.Application.Queries.GetUserByEmail;
using UsersAPI.Application.Queries.GetUserById;
using UsersAPI.Application.Queries.GetUserByName;
using UsersAPI.Application.Queries.GetUsers;
using UsersAPI.Domain.Common;

namespace UsersAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAppLogger<UsersController> _logger;

        public UsersController(
            IMediator mediator,
            IAppLogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <remarks>
        /// Creates a new user account in the system.
        ///
        /// This endpoint is public and does not require authentication.
        /// </remarks>
        /// <param name="model">User registration data</param>
        /// <param name="cancellationToken">Request cancellation token</param>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Validation or business rule error</response>
        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterUserCommand model,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(model, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError($"Register user failed: {result.Message}");
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Authenticates a user
        /// </summary>
        /// <remarks>
        /// Authenticates a user and returns an access token.
        /// </remarks>
        /// <param name="model">Login credentials</param>
        /// <param name="cancellationToken">Request cancellation token</param>
        /// <response code="200">User authenticated successfully</response>
        /// <response code="400">Invalid credentials</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(
            [FromBody] NewLoginCommand model,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(model, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError($"Login failed: {result.Message}");
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a paginated list of users
        /// </summary>
        /// <remarks>
        /// Returns all users in a paginated format.
        ///
        /// Admin access required.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Get(
            [FromQuery] GetUsersQuery model,
            CancellationToken ct)
        {
            var result = await _mediator.Send(model, ct);

            if (!result.IsSuccess)
            {
                _logger.LogError($"Failed to list users. Reason: {result.Message}");
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a user by email
        /// </summary>
        /// <remarks>
        /// Admin access required.
        /// </remarks>
        [HttpGet("email")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmail(
            [FromQuery] string email,
            CancellationToken ct)
        {
            var result = await _mediator.Send(new GetUserByEmailQuery(email), ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning($"User not found. Email: {email}");
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves users by name
        /// </summary>
        /// <remarks>
        /// Admin access required.
        /// </remarks>
        [HttpGet("name")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByName(
            [FromQuery] string name,
            CancellationToken ct)
        {
            var result = await _mediator.Send(new GetUserByNameQuery(name), ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning($"No users found. Name: {name}");
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves a user by ID
        /// </summary>
        /// <remarks>
        /// Admin access required.
        /// </remarks>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken ct)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id), ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning($"User not found. UserId: {id}");
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Updates a user
        /// </summary>
        /// <remarks>
        /// Updates user data.
        ///
        /// Admin access required.
        /// </remarks>
        [HttpPatch("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateUserRequest body,
            CancellationToken ct)
        {
            var cmd = new UpdateUserCommand(id, body.Name, body.Email, body.IsActive);

            var result = await _mediator.Send(cmd, ct);

            if (!result.IsSuccess)
            {
                _logger.LogError($"Failed to update user. UserId: {id}. Reason: {result.Message}");
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Changes a user's role
        /// </summary>
        /// <remarks>
        /// Admin access required.
        /// </remarks>
        [HttpPatch("{id:guid}/role")]
        [Authorize(Roles = "Admin")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeRole(
            Guid id,
            [FromBody] ChangeUserRoleRequest body,
            CancellationToken ct)
        {
            var cmd = new ChangeUserRoleCommand(id, body.Role);

            var result = await _mediator.Send(cmd, ct);

            if (!result.IsSuccess)
            {
                _logger.LogError($"Failed to change user role. UserId: {id}. Reason: {result.Message}");
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <remarks>
        /// Permanently deletes a user.
        ///
        /// Admin access required.
        /// </remarks>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteUserCommand(id), ct);

            if (!result.IsSuccess)
            {
                _logger.LogError($"Failed to delete user. UserId: {id}. Reason: {result.Message}");
                return BadRequest(result.Message);
            }

            return NoContent();
        }
        public record ChangeUserRoleRequest(string Role);

        public record UpdateUserRequest(
            string? Name = null,
            string? Email = null,
            bool? IsActive = null
        );
    }
}
