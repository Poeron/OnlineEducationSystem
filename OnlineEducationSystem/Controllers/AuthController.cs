using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Npgsql;

namespace OnlineEducationSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    private string GenerateToken(Users user)
    {
        var claims = new[]
        {
                new Claim("user_id", user.user_id.ToString()),
                new Claim("name", user.name),
                new Claim("role", user.role)
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO user)
    {
        var query = "SELECT user_id,role,name FROM users WHERE email = @email AND password = @password";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@email", user.email),
            new NpgsqlParameter("@password", user.password)
        };

        var result = _dbHelper.ExecuteReader(query, reader => new 
        {
            user_id = reader.GetInt32(0),
            role = reader.GetString(1),
            name = reader.GetString(2)
        }, parameters).FirstOrDefault();

        if (result == null)
        {
            return Unauthorized();
        }

        var token = GenerateToken(new Users
        {
            user_id = result.user_id,
            role = result.role,
            name = result.name,
        });

        return Ok(new { Token = token, Role = result.role });

    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDTO user)
    {
        var query = "INSERT INTO users (email, password, role, name) VALUES (@email, @password, @role, @name)";
        var parameters = new[]
        {
            new NpgsqlParameter("@email", user.email),
            new NpgsqlParameter("@password", user.password),
            new NpgsqlParameter("@role", user.role),
            new NpgsqlParameter("@name", user.name)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        return Ok(new { message = "Kayıt Başarıyla Oluşturuldu." });
    }
}
