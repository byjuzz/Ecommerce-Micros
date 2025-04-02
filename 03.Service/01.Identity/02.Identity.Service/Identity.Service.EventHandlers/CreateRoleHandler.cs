using Identity.Domain;
using Identity.Service.EventHandlers.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Service.EventHandlers
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, bool>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public CreateRoleHandler(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (await _roleManager.RoleExistsAsync(request.RoleName))
                return false;

            var role = new ApplicationRole { Name = request.RoleName };
            var result = await _roleManager.CreateAsync(role);
            return result.Succeeded;
        }
    }
}
