using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotnetStoreAPI.Models;
using DotnetStoreAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace DotnetStoreAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{

    private readonly ApplicationDbContext _context;

    //Create object for manage user

    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly IConfiguration _configuration;



    public AuthController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }


    [HttpGet("testconnectdb")]
    public void TestConnect()
    {
        if (_context.Database.CanConnect())
        {
            Response.WriteAsync("Connected");
        }
        else
        {
            Response.WriteAsync("Not Connected");
        }
    }


    [HttpPost("register-user")]
    public async Task<ActionResult> RegisterUser([FromBody] RegisteModel model)
    {
        var user = new IdentityUser
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new ResponseModel
            {
                Status = "Error",
                Message = "User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }

        if (!await _roleManager.RoleExistsAsync(UserRolesModel.User))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRolesModel.User));
            await _userManager.AddToRoleAsync(user, UserRolesModel.User);
        }

        return Ok(new ResponseModel
        {
            Status = "Success",
            Message = "User registered successfully"
        });
    }


    [HttpPost("login-user")]

    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {

        var user = await _userManager.FindByNameAsync(login.Username!);

        if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GetToken(authClaims);

            return Ok(
              new
              {
                  token = new JwtSecurityTokenHandler().WriteToken(token),
                  expiration = token.ValidTo,
                  userData = new
                  {
                      userName = user.UserName,
                      email = user.Email,
                      roles = userRoles
                  }
              }
            );
        }

        return Ok();

    }



    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var authSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(24),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSignKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}
