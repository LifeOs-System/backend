namespace Domain.Entities.Users;

public interface IUserRepository
{
    public Task<User?> GetUserAsync(Guid id);
    public Task CreateUserAsync(User user);
}
