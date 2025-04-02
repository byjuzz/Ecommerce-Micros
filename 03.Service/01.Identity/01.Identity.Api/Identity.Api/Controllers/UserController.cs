using Identity.Domain;
using Identity.Service.Queries.Commands;
using Identity.Service.Queries.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceCommon.Collection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ILogger<UserController> logger, IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<DataCollection<UserDto>> GetAll(int page = 1, int take = 10, string ids = null)
        {
            var users = ids?.Split(',');
            return await _mediator.Send(new GetUsersQuery(page, take, users));
        }

        [HttpGet("{id}")]
        public async Task<UserDto> Get(string id)
        {
            return await _mediator.Send(new GetUserQuery(id));
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest("No se pudo extraer el ID del usuario desde el token.");

            var user = await _mediator.Send(new GetUserQuery(userId));

            if (user == null)
                return NotFound("Usuario no encontrado");

            // ⚠️ Aquí recuperamos los roles del usuario
            var appUser = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(appUser);

            // Los incluimos en el DTO
            user.Roles = roles.ToList();

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token"); // ✅ Borra el JWT
            return Ok(new { message = "Sesión cerrada correctamente" });
        }
    }
}
