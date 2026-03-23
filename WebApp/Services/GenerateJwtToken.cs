using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Services;

public class GenerateJwtToken
{
    private static DateTime TokenExpirationTime { get; set; }
    private static DateTime CurrentTime { get; set; }

    public Task<string> GenerateJwtTokenmethod(string username)
    {
        var key = Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharsLong"); // Use same key from config
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, username)
        };

        var token = new JwtSecurityToken(
            "yourdomain.com",
            "yourdomain.com",
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public static async Task<(DateTime TokenExpirationTime, DateTime CurrentTime)> CalculateExpirationTime()
    {
        CurrentTime = DateTime.UtcNow;
        TokenExpirationTime = CurrentTime.AddHours(1);

        return await Task.FromResult((TokenExpirationTime, CurrentTime));
    }
}