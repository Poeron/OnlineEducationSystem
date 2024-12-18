using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExamQuestionsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public ExamQuestionsController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    [HttpGet]
    public IActionResult GetExamQuestions()
    {
        var query = "SELECT * FROM ExamQuestions";
        var questions = _dbHelper.ExecuteReader(query, reader => new ExamQuestions
        {
            question_id = reader.GetInt32(0),
            exam_id = reader.GetInt32(1),
            question_text = reader.GetString(2),
            question_type = reader.GetString(3),
            created_at = reader.GetDateTime(4),
            updated_at = reader.GetDateTime(5),
            deleted_at = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
        }).Where(question => question.deleted_at == null).ToList();

        return Ok(questions);
    }



    [HttpGet("{id}")]
    public IActionResult GetExamQuestion(int id)
    {
        var query = "SELECT * FROM ExamQuestions WHERE exam_id = @id AND deleted_at IS NULL";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var questions = _dbHelper.ExecuteReader(query, reader => new ExamQuestions
        {
            question_id = reader.GetInt32(0),
            exam_id = reader.GetInt32(1),
            question_text = reader.GetString(2),
            question_type = reader.GetString(3),
            created_at = reader.GetDateTime(4),
            updated_at = reader.GetDateTime(5),
            deleted_at = reader.IsDBNull(6) ? null : reader.GetDateTime(6)
        }, parameters).ToList();


        return Ok(questions);
    }

    [Authorize(Roles = "instructor, admin")]
    [HttpPost]
    public IActionResult CreateExamQuestion([FromBody] CreateExamQuestions question)
    {
        var query = "INSERT INTO ExamQuestions (exam_id, question_text, question_type) VALUES (@exam_id, @question_text, @question_type) RETURNING question_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@exam_id", question.exam_id),
            new NpgsqlParameter("@question_text", question.question_text),
            new NpgsqlParameter("@question_type", question.question_type)
        };

        var insertedQuestion = _dbHelper.ExecuteReader(query, reader => new
        {
            question_id = reader.GetInt32(0) // RETURNING yalnızca question_id döndürüyor
        }, parameters).FirstOrDefault();

        return Ok(insertedQuestion!.question_id);
    }

    [Authorize(Roles = "instructor, admin")]
    [HttpPatch]
    public IActionResult UpdateExamQuestion([FromBody] PatchExamQuestions question)
    {
        var query = "UPDATE ExamQuestions SET question_text = @question_text, question_type = @question_type WHERE question_id = @question_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@question_id", question.question_id),
            new NpgsqlParameter("@question_text", question.question_text),
            new NpgsqlParameter("@question_type", question.question_type)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Güncelleme Başarılı" });
    }

    [Authorize(Roles = "instructor,admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteExamQuestion(int id)
    {
        var query = "UPDATE ExamQuestions SET deleted_at = @deleted_at WHERE question_id = @question_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@question_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Kayıt Başarıyla Silindi." });
    }
}
