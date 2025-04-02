
using Identity.Persistence.Database;
using Identity.Service.Queries.Commands;
using Identity.Service.Queries.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceCommon.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Service.Queries
{
    public class UserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserQueryHandler(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _applicationDbContext.ApplicationUsers
                .Include(x => x.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .SingleAsync(x => x.Id == request.Id, cancellationToken);

            var dto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            return dto;
        }
    }
}
