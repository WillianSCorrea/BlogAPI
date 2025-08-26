using BlogApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlogApi.Services;

public class AuthServices
{
    private readonly IConfiguration _config;
    private readonly BlogDbContext _context;

    public AuthServices(IConfiguration config , BlogDbContext context)
    {
        _config = config; _context = context;
    }

    public string GenerateJwt(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
        };
        var key = new SymmeteicSecurityKey(Encoding.UFT8.getBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgoristihms.HmacSha256);

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
