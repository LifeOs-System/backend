using Application.Services.Users.Create;

namespace Application.Services.Users;

public interface IUserService
{
    public Task CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
}
