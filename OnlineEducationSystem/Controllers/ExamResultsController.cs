using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ExamResultsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public ExamResultsController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    [HttpGet]
    public IActionResult GetExamResults()
    {
        var query = "SELECT * FROM ExamResults";
        var results = _dbHelper.ExecuteReader(query, reader => new ExamResults
        {
            result_id = reader.GetInt32(0),
            exam_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            score = reader.GetInt32(3),
            taken_at = reader.GetDateTime(4),
            deleted_at = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
        }).Where(result => result.deleted_at == null).ToList();

        return Ok(results);
    }

    [HttpGet("course/{course_id}/student/{student_id}")]
    public IActionResult GetExamResult(int course_id, int student_id)
    {
        var query = @"
        SELECT er.* FROM examResults er INNER JOIN exams e ON er.exam_id = e.exam_id
        WHERE e.course_id = @course_id AND er.student_id = @student_id AND er.deleted_at IS NULL";

        var parameters = new NpgsqlParameter[]
        {
        new NpgsqlParameter("@course_id", course_id),
        new NpgsqlParameter("@student_id", student_id)
        };

        var result = _dbHelper.ExecuteReader(query, reader => new ExamResults
        {
            result_id = reader.GetInt32(0),
            exam_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            score = reader.GetInt32(3),
            taken_at = reader.GetDateTime(4),
            deleted_at = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
        },parameters).FirstOrDefault();

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }


    [HttpGet("{id}")]
    public IActionResult GetExamResult(int id)
    {
        var query = "SELECT * FROM ExamResults WHERE result_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var result = _dbHelper.ExecuteReader(query, reader => new ExamResults
        {
            result_id = reader.GetInt32(0),
            exam_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            score = reader.GetInt32(3),
            taken_at = reader.GetDateTime(4),
            deleted_at = reader.IsDBNull(5) ? null : reader.GetDateTime(5)
        }, parameters).FirstOrDefault();

        if (result == null || result.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize(Roles = "student, admin")]
    [HttpPost]
    public IActionResult CreateExamResult([FromBody] CreateExamResults result)
    {
        var query = "INSERT INTO ExamResults (exam_id, student_id, score, taken_at) VALUES (@exam_id, @student_id, @score, NOW())";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@exam_id", result.exam_id),
            new NpgsqlParameter("@student_id", result.student_id),
            new NpgsqlParameter("@score", result.score)
        };

        var resultId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(resultId);
    }

    [Authorize(Roles = "instructor, admin")]
    [HttpPatch]
    public IActionResult UpdateExamResult([FromBody] PatchExamResults result)
    {
        var query = "UPDATE ExamResults SET score = @score WHERE result_id = @result_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@result_id", result.result_id),
            new NpgsqlParameter("@score", result.score)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Güncelleme Başarılı" });
    }

    [Authorize(Roles = "instructor,admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteExamResult(int id)
    {
        var query = "UPDATE ExamResults SET deleted_at = @deleted_at WHERE result_id = @result_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@result_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Kayıt Başarıyla Silindi." });
    }
}
