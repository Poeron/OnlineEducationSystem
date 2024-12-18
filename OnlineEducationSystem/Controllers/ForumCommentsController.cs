using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ForumCommentsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public ForumCommentsController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    [HttpGet]
    public IActionResult GetForumComments()
    {
        var query = "SELECT * FROM ForumComments";
        var comments = _dbHelper.ExecuteReader(query, reader => new ForumComments
        {
            comment_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            author_id = reader.IsDBNull(2) ? null : reader.GetInt32(2),
            comment_text = reader.GetString(3),
            created_at = reader.GetDateTime(4),
            updated_at = reader.GetDateTime(5),
            deleted_at = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
        }).Where(comment => comment.deleted_at == null).ToList();

        return Ok(comments);
    }

    [HttpGet("Course/{course_id}")]
    public IActionResult GetCommentsForCourse(int course_id)
    {
        var query = "SELECT fc.comment_id, fc.comment_text, fc.created_at, u.name FROM ForumComments fc INNER JOIN Users u ON u.user_id = fc.author_id  WHERE course_id = @course_id AND fc.deleted_at IS NULL ORDER BY fc.created_at ASC";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", course_id)
        };

        var comments = _dbHelper.ExecuteReader(query, reader => new ViewComments
        {
            comment_id = reader.GetInt32(0),
            comment_text = reader.GetString(1),
            created_at = reader.GetDateTime(2),
            author_name = reader.GetString(3)
        }, parameters);

        return Ok(comments);
    }

    [HttpGet("{id}")]
    public IActionResult GetForumComment(int id)
    {
        var query = "SELECT * FROM ForumComments WHERE comment_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var comment = _dbHelper.ExecuteReader(query, reader => new ForumComments
        {
            comment_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            author_id = reader.IsDBNull(2) ? null : reader.GetInt32(2),
            comment_text = reader.GetString(3),
            created_at = reader.GetDateTime(4),
            updated_at = reader.GetDateTime(5),
            deleted_at = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
        }, parameters).FirstOrDefault();

        if (comment == null || comment.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(comment);
    }

    [Authorize]
    [HttpPost]
    public IActionResult CreateForumComment([FromBody] CreateForumComments comment)
    {
        var query = "INSERT INTO ForumComments (course_id, author_id, comment_text) VALUES (@course_id, @author_id, @comment_text)";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", comment.course_id),
            new NpgsqlParameter("@author_id", comment.author_id),
            new NpgsqlParameter("@comment_text", comment.comment_text)
        };

        var commentId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(commentId);
    }

    [Authorize]
    [HttpPatch]
    public IActionResult UpdateForumComment([FromBody] PatchForumComments comment)
    {
        var query = "UPDATE ForumComments SET comment_text = @comment_text WHERE comment_id = @comment_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@comment_id", comment.comment_id),
            new NpgsqlParameter("@comment_text", comment.comment_text)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Güncelleme Başarılı" });
    }

    [Authorize(Roles = "instructor, admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteForumComment(int id)
    {
        var query = "UPDATE ForumComments SET deleted_at = @deleted_at WHERE comment_id = @comment_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@comment_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Kayıt Başarıyla Silindi." });
    }
}
