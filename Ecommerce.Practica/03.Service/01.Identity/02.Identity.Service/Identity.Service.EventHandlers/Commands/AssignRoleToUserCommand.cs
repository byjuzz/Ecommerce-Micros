using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Service.EventHandlers.Commands
{
    public class AssignRoleToUserCommand : IRequest<bool>
    {
        public string UserId { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }

}
