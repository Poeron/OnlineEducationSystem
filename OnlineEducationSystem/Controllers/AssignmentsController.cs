using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AssignmentsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public AssignmentsController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    [HttpGet]
    public IActionResult GetAssignments()
    {
        var query = "SELECT * FROM assignments";
        var assignments = _dbHelper.ExecuteReader(query, reader => new Assignments
        {
            assignment_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            title = reader.GetString(2),
            description = reader.IsDBNull(3) ? null : reader.GetString(3),
            due_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
            created_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }).Where(assignment => assignment.deleted_at == null).ToList();

        return Ok(assignments);
    }

    [HttpGet("Courses/{course_id}")]
    public IActionResult GetAssignmentsForCourse(int course_id)
    {
        var query = "SELECT * FROM assignments WHERE course_id = @course_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", course_id)
        };

        var assignments = _dbHelper.ExecuteReader(query, reader => new Assignments
        {
            assignment_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            title = reader.GetString(2),
            description = reader.IsDBNull(3) ? null : reader.GetString(3),
            due_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
            created_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }, parameters).Where(assignment => assignment.deleted_at == null).ToList();

        return Ok(assignments);
    }

    [HttpGet("Courses/{course_id}/student/{student_id}")]
    public IActionResult GetAssignmentsForCourseAndStudent(int course_id, int student_id)
    {
        var query = @"
        SELECT a.*
        FROM assignments a
        INNER JOIN courseEnrollments ce ON a.course_id = ce.course_id
        WHERE a.course_id = @course_id AND ce.student_id = @student_id AND a.deleted_at IS NULL";

        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", course_id),
            new NpgsqlParameter("@student_id", student_id)
        };

        var assignments = _dbHelper.ExecuteReader(query, reader => new Assignments
        {
            assignment_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            title = reader.GetString(2),
            description = reader.IsDBNull(3) ? null : reader.GetString(3),
            due_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
            created_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }, parameters).Where(assignment => assignment.deleted_at == null).ToList();

        return Ok(assignments);
    }


    [HttpGet("{id}")]
    public IActionResult GetAssignment(int id)
    {
        var query = "SELECT * FROM assignments WHERE assignment_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var assignment = _dbHelper.ExecuteReader(query, reader => new Assignments
        {
            assignment_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            title = reader.GetString(2),
            description = reader.IsDBNull(3) ? null : reader.GetString(3),
            due_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
            created_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }, parameters).FirstOrDefault();

        if (assignment == null || assignment.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(assignment);
    }

    [Authorize(Roles = "instructor")]
    [HttpPost]
    public IActionResult CreateAssignment([FromBody] CreateAssignments assignment)
    {
        var query = "INSERT INTO assignments (course_id, title, description, due_date) VALUES (@course_id, @title, @description, @due_date)";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", assignment.course_id),
            new NpgsqlParameter("@title", assignment.title),
            new NpgsqlParameter("@description", assignment.description ?? (object)DBNull.Value),
            new NpgsqlParameter("@due_date", assignment.due_date ?? (object)DBNull.Value)
        };

        var assignmentId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(assignmentId);
    }

    [Authorize(Roles = "instructor")]
    [HttpPatch]
    public IActionResult UpdateAssignment([FromBody] PatchAssignments assignment)
    {
        var query = "UPDATE assignments SET title = @title, description = @description, due_date = @due_date WHERE assignment_id = @assignment_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@assignment_id", assignment.assignment_id),
            new NpgsqlParameter("@title", assignment.title),
            new NpgsqlParameter("@description", assignment.description ?? (object)DBNull.Value),
            new NpgsqlParameter("@due_date", assignment.due_date ?? (object)DBNull.Value)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok();
    }

    [Authorize(Roles = "instructor,admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteAssignment(int id)
    {
        var query = "UPDATE assignments SET deleted_at = @deleted_at WHERE assignment_id = @assignment_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@assignment_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok();
    }
}
