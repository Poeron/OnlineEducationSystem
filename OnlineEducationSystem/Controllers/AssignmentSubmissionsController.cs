using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AssignmentSubmissionsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public AssignmentSubmissionsController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    [HttpGet]
    public IActionResult GetAssignmentSubmissions()
    {
        var query = "SELECT * FROM AssignmentSubmissions";
        var submissions = _dbHelper.ExecuteReader(query, reader => new AssignmentSubmissions
        {
            submission_id = reader.GetInt32(0),
            assignment_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            submission_url = reader.IsDBNull(3) ? null : reader.GetString(3),
            grade = reader.IsDBNull(4) ? null : reader.GetInt32(4),
            submitted_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }).Where(submission => submission.deleted_at == null).ToList();

        return Ok(submissions);
    }

    [HttpGet("{id}")]
    public IActionResult GetAssignmentSubmission(int id)
    {
        var query = "SELECT * FROM AssignmentSubmissions WHERE submission_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var submission = _dbHelper.ExecuteReader(query, reader => new AssignmentSubmissions
        {
            submission_id = reader.GetInt32(0),
            assignment_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            submission_url = reader.IsDBNull(3) ? null : reader.GetString(3),
            grade = reader.IsDBNull(4) ? null : reader.GetInt32(4),
            submitted_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }, parameters).FirstOrDefault();

        if (submission == null || submission.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(submission);
    }

    [Authorize]
    [HttpPost]
    public IActionResult CreateAssignmentSubmission([FromBody] CreateAssignmentSubmissions submission)
    {
        var query = "INSERT INTO AssignmentSubmissions (assignment_id, student_id, submission_url) VALUES (@assignment_id, @student_id, @submission_url)";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@assignment_id", submission.assignment_id),
            new NpgsqlParameter("@student_id", submission.student_id),
            new NpgsqlParameter("@submission_url", submission.submission_url)
        };

        var submissionId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(submissionId);
    }

    [Authorize(Roles = "instructor")]
    [HttpPatch]
    public IActionResult UpdateAssignmentSubmission([FromBody] PatchAssignmentSubmissions submission)
    {
        var query = "UPDATE AssignmentSubmissions SET grade = @grade WHERE submission_id = @submission_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@submission_id", submission.submission_id),
            new NpgsqlParameter("@grade", submission.grade)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok();
    }

    [Authorize(Roles = "admin,instructor")]
    [HttpDelete("{id}")]
    public IActionResult DeleteAssignmentSubmission(int id)
    {
        var query = "UPDATE AssignmentSubmissions SET deleted_at = @deleted_at WHERE submission_id = @submission_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@submission_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok();
    }
}
