using Identity.Service.Queries.DTOs;
using MediatR;
using ServiceCommon.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Service.Queries.Commands
{

    public record GetUsersQuery(int Page, int Take, IEnumerable<string> Users = null) : IRequest<DataCollection<UserDto>>;

    public record GetUserQuery(string Id) : IRequest<UserDto>;


}
