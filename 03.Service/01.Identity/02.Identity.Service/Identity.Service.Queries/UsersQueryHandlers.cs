using Identity.Persistence.Database;
using Identity.Service.Queries.Commands;
using Identity.Service.Queries.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceCommon.Collection;
using ServiceCommon.Mapping;
using ServiceCommon.Paging;

namespace Identity.Service.Queries
{
    public class UsersQueryHandlers : IRequestHandler<GetUsersQuery, DataCollection<UserDto>>
    {
        public readonly ApplicationDbContext _applicationDbContext;

        public UsersQueryHandlers(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<DataCollection<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var collection = await _applicationDbContext.ApplicationUsers
                .Include(x => x.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(x => request.Users == null || request.Users.Contains(x.Id))
                .OrderByDescending(x => x.FirstName)
                .GetPagedAsync(request.Page, request.Take);

            var dtos = collection.Items.Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).ToList();

            return new DataCollection<UserDto>
            {
                Items = dtos,
                Total = collection.Total,
                Page = collection.Page,
                Pages = collection.Pages
            };
        }
    }
}



