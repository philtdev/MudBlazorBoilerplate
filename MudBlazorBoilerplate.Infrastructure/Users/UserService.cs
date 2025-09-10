namespace MudBlazorBoilerplate.Infrastructure.Users;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _roleManager = roleManager;
    }

    public async Task AddRoleToUserAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            throw new Exception("User not found.");

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (!roleResult.Succeeded)
                throw new Exception("Failed to create role.");
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);

        if (!result.Succeeded)
            throw new Exception("Failed to add role to user.");
    }

    public async Task<string> GetCurrentUserIdAsync()
    {
        var user = await GetCurrentUserAsync();

        if (user is null)
            throw new UserNotAuthorizedException();

        return user.Id;
    }

    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return [];

        var roles = await _userManager.GetRolesAsync(user);

        return roles.ToList();
    }

    public async Task<bool> IsCurrentUserInRoleAsync(string role)
    {
        var user = await GetCurrentUserAsync();
        var result = user is not null && await _userManager.IsInRoleAsync(user, role);

        return result;
    }

    public async Task RemoveRoleFromUserAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            throw new Exception("User not found.");

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (!result.Succeeded)
            throw new Exception("Failed to remove role from user.");
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null || httpContext.User is null)
            return null;

        var user = await _userManager.GetUserAsync(httpContext.User);

        return user;
    }
}
