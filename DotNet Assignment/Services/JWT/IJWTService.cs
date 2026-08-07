using DotNet_Assignment.Models.Entities;

namespace DotNet_Assignment.Services.JWT
{
    public interface IJWTService
    {
        string GetAccessToken(User user);
        string GenerateRefreshToken();
    }
}
