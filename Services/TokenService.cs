using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DotnetStoreAPI.Services;

public class TokenService {

    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration){
        _configuration = configuration;
    }

    public JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(24),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSignKey,SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}