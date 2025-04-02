using Identity.Service.EventHandlers.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("admin/users")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminUsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleToUserCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success) return BadRequest("No se pudo asignar el rol");
            return Ok(new { message = "Rol asignado exitosamente" });
        }
        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success) return BadRequest("No se pudo crear el rol (ya existe o hubo un error)");
            return Ok(new { message = "Rol creado exitosamente" });
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 🔍 Esto mostrará en Postman o consola qué campo está mal
            }

            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors); // También podés ver el error exacto de Identity
            }

            return Ok(new { message = "Usuario creado exitosamente" });
        }
    }
}
