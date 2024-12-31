using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public UsersController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    // GET: api/Users
    [Authorize(Roles = "admin")]
    [HttpGet]
    public IActionResult GetUsers()
    {
        var query = "SELECT * FROM Users ORDER BY user_id ASC";
        var users = _dbHelper.ExecuteReader(query, reader => new Users
        {
            user_id = reader.GetInt32(0),
            email = reader.GetString(1),
            password = reader.GetString(2), // Avoid exposing passwords in real-world scenarios
            role = reader.GetString(3),
            name = reader.GetString(4),
            created_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }).Where(user => user.deleted_at == null).ToList();

        return Ok(users);
    }

    // GET: api/Users/{id}
    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var query = "SELECT * FROM Users WHERE user_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var user = _dbHelper.ExecuteReader(query, reader => new Users
        {
            user_id = reader.GetInt32(0),
            email = reader.GetString(1),
            password = reader.GetString(2), // Avoid exposing passwords
            role = reader.GetString(3),
            name = reader.GetString(4),
            created_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }, parameters).FirstOrDefault();

        if (user == null || user.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // POST: api/Users
    [Authorize(Roles = "admin")]
    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUser user)
    {
        var query = "INSERT INTO Users (email, password, role, name) VALUES (@email, @password, @role, @name)";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password);
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@email", user.email),
            new NpgsqlParameter("@password", hashedPassword),
            new NpgsqlParameter("@role", user.role),
            new NpgsqlParameter("@name", user.name)
        };

        var userId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(userId);
    }

    // PATCH: api/Users
    [Authorize]
    [HttpPatch]
    public IActionResult UpdateUser([FromBody] UpdateUser user)
    {
        var query = "UPDATE Users SET name = @name, role = @role, email = @email, password = @password WHERE user_id = @user_id";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.password);
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@user_id", user.user_id),
            new NpgsqlParameter("@name", user.name),
            new NpgsqlParameter("@email", user.email),
            new NpgsqlParameter("@role", user.role),
            new NpgsqlParameter("@password", hashedPassword)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Güncelleme Başarılı" });
    }

    // DELETE: api/Users/{id}
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var query = "UPDATE Users SET deleted_at = @deleted_at WHERE user_id = @user_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@user_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new {message = "Kayıt Başarıyla Silindi."});
    }
}
