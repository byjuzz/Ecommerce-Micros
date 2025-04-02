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
    public class AdminCreateUserEventHandler : IRequestHandler<AdminCreateUserCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminCreateUserEventHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(AdminCreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,  // 👈 ¿Está esta línea?
                LastName = request.LastName     // 👈 ¿Y esta también?
            };

            return await _userManager.CreateAsync(user, request.Password);
        }
    }

}