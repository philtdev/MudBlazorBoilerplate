namespace MudBlazorBoilerplate.Application.Users;

public interface IUserService
{
    Task<string> GetCurrentUserIdAsync();
    Task<bool> IsCurrentUserInRoleAsync(string role);
    Task<List<string>> GetUserRolesAsync(string userId);
    Task AddRoleToUserAsync(string userId, string roleName);
    Task RemoveRoleFromUserAsync(string userId, string roleName);
}
