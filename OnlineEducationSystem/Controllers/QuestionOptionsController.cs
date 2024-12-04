using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionOptionsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public QuestionOptionsController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    [HttpGet]
    public IActionResult GetQuestionOptions()
    {
        var query = "SELECT * FROM QuestionOptions";
        var options = _dbHelper.ExecuteReader(query, reader => new QuestionOptions
        {
            option_id = reader.GetInt32(0),
            question_id = reader.GetInt32(1),
            option_text = reader.GetString(2),
            is_correct = reader.GetBoolean(3),
            created_at = reader.GetDateTime(4),
            updated_at = reader.GetDateTime(5),
            deleted_at = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
        }).Where(option => option.deleted_at == null).ToList();

        return Ok(options);
    }

    [HttpGet("question/{question_id}")]
    public IActionResult GetQuestionOptionsForQuestion(int question_id)
    {
        var query = "SELECT * FROM QuestionOptions WHERE question_id = @question_id";
        var parameters = new NpgsqlParameter[]
        {
         new NpgsqlParameter("@question_id", question_id)
        };

        var options = _dbHelper.ExecuteReader(query, reader => new QuestionOptions
        {
            option_id = reader.GetInt32(0),
            question_id = reader.GetInt32(1),
            option_text = reader.GetString(2),
            is_correct = reader.GetBoolean(3),
            created_at = reader.GetDateTime(4),
            updated_at = reader.GetDateTime(5),
            deleted_at = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
        }, parameters).Where(option => option.deleted_at == null).ToList();

        return Ok(options);
    }

    [HttpGet("{id}")]
    public IActionResult GetQuestionOption(int id)
    {
        var query = "SELECT * FROM QuestionOptions WHERE option_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var option = _dbHelper.ExecuteReader(query, reader => new QuestionOptions
        {
            option_id = reader.GetInt32(0),
            question_id = reader.GetInt32(1),
            option_text = reader.GetString(2),
            is_correct = reader.GetBoolean(3),
            created_at = reader.GetDateTime(4),
            updated_at = reader.GetDateTime(5),
            deleted_at = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
        }, parameters).FirstOrDefault();

        if (option == null || option.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(option);
    }

    [Authorize(Roles = "instructor, admin")]
    [HttpPost]
    public IActionResult CreateQuestionOption([FromBody] CreateQuestionOptions option)
    {
        var query = "INSERT INTO QuestionOptions (question_id, option_text, is_correct) VALUES (@question_id, @option_text, @is_correct) RETURNING option_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@question_id", option.question_id),
            new NpgsqlParameter("@option_text", option.option_text),
            new NpgsqlParameter("@is_correct", option.is_correct)
        };

        var optionId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(optionId);
    }

    [Authorize(Roles = "instructor, admin")]
    [HttpPatch]
    public IActionResult UpdateQuestionOption([FromBody] PatchQuestionOptions option)
    {
        var query = "UPDATE QuestionOptions SET option_text = @option_text, is_correct = @is_correct WHERE option_id = @option_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@option_id", option.option_id),
            new NpgsqlParameter("@option_text", option.option_text),
            new NpgsqlParameter("@is_correct", option.is_correct)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Güncelleme Başarılı" });
    }

    [Authorize(Roles = "instructor,admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteQuestionOption(int id)
    {
        var query = "UPDATE QuestionOptions SET deleted_at = @deleted_at WHERE option_id = @option_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@option_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Kayıt Başarıyla Silindi." });
    }
}
