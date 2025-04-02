using Identity.Domain;
using Identity.Service.EventHandlers.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("identity")]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IMediator _mediator;

        public IdentityController(ILogger<IdentityController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateCommand command)
        {
            if (!ModelState.IsValid) return BadRequest();
            var result = await _mediator.Send(command);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(new { message = "Usuario creado correctamente" });
        }

        [HttpPost("authentication")]
        public async Task<IActionResult> Login(UserLoginCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await _mediator.Send(command);

            if (!result.Succeded)
                return Unauthorized("Credenciales inválidas");

            // ✅ Guardar token en cookie HttpOnly
            Response.Cookies.Append("access_token", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // 👈 CAMBIAR A FALSE EN DESARROLLO
                SameSite = SameSiteMode.Lax, // o podés probar Lax si no tenés frontend en otro dominio
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Ok(new { message = "Login exitoso" });
        }
    }
}
