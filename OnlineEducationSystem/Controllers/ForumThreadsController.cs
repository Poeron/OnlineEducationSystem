using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ForumThreadsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public ForumThreadsController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    [HttpGet]
    public IActionResult GetForumThreads()
    {
        var query = "SELECT * FROM ForumThreads";
        var threads = _dbHelper.ExecuteReader(query, reader => new ForumThreads
        {
            thread_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            author_id = reader.IsDBNull(2) ? null : reader.GetInt32(2),
            title = reader.GetString(3),
            created_at = reader.GetDateTime(4),
            updated_at = reader.GetDateTime(5),
            deleted_at = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
        }).Where(thread => thread.deleted_at == null).ToList();

        return Ok(threads);
    }

    [HttpGet("{id}")]
    public IActionResult GetForumThread(int id)
    {
        var query = "SELECT * FROM ForumThreads WHERE thread_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var thread = _dbHelper.ExecuteReader(query, reader => new ForumThreads
        {
            thread_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            author_id = reader.IsDBNull(2) ? null : reader.GetInt32(2),
            title = reader.GetString(3),
            created_at = reader.GetDateTime(4),
            updated_at = reader.GetDateTime(5),
            deleted_at = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
        }, parameters).FirstOrDefault();

        if (thread == null || thread.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(thread);
    }

    [Authorize]
    [HttpPost]
    public IActionResult CreateForumThread([FromBody] CreateForumThreads thread)
    {
        var query = "INSERT INTO ForumThreads (course_id, author_id, title) VALUES (@course_id, @author_id, @title)";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", thread.course_id),
            new NpgsqlParameter("@author_id", thread.author_id),
            new NpgsqlParameter("@title", thread.title)
        };

        var threadId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(threadId);
    }

    [Authorize]
    [HttpPatch]
    public IActionResult UpdateForumThread([FromBody] PatchForumThreads thread)
    {
        var query = "UPDATE ForumThreads SET title = @title WHERE thread_id = @thread_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@thread_id", thread.thread_id),
            new NpgsqlParameter("@title", thread.title)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok();
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteForumThread(int id)
    {
        var query = "UPDATE ForumThreads SET deleted_at = @deleted_at WHERE thread_id = @thread_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@thread_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok();
    }
}
