using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;
using System.IdentityModel.Tokens.Jwt;

namespace OnlineEducationSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseEnrollmentsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public CourseEnrollmentsController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    [Authorize(Roles = "instructor,admin")]
    [HttpGet]
    public IActionResult GetCourseEnrollments(int course_id)
    {
        var query = "SELECT * FROM courseEnrollments WHERE course_id = @course_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", course_id)
        };
        var courseEnrollments = _dbHelper.ExecuteReader(query, reader => new CourseEnrollments
        {
            enrollment_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            enrollment_date = reader.GetDateTime(3),
            deleted_at = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
        }, parameters);
        // remove deleted course enrollments
        courseEnrollments = courseEnrollments.Where(courseEnrollment => courseEnrollment.deleted_at == null).ToList();
        if (courseEnrollments.Count == 0)
        {
            return NotFound();
        }
        return Ok(courseEnrollments);
    }
    [Authorize(Roles = "instructor,admin")]
    [HttpGet("{id}")]
    public IActionResult GetCourseEnrollment(int id)
    {
        var query = "SELECT * FROM courseEnrollments WHERE enrollment_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var courseEnrollment = _dbHelper.ExecuteReader(query, reader => new CourseEnrollments
        {
            enrollment_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            enrollment_date = reader.GetDateTime(3),
            deleted_at = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
        }, parameters).FirstOrDefault();

        if (courseEnrollment == null || courseEnrollment.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(courseEnrollment);

    }

    [Authorize(Roles = "student")]
    [HttpPost]
    public IActionResult CreateCourseEnrollment([FromBody] CreateCourseEnrollments course)
    {
        // Get the JWT bearer token from the request headers
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        // Decode the JWT bearer token to get the user id
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "user_id")!.Value;
        // Use the user id as the student id in the course enrollment
        int student_id = int.Parse(userId);

        var query = "INSERT INTO courseEnrollments (course_id, student_id) VALUES (@course_id, @student_id)";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", course.course_id),
            new NpgsqlParameter("@student_id", student_id)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        return Ok();
    }

    [Authorize(Roles = "instructor,admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteCourseEnrollment(int id)
    {
        var query = "UPDATE courseEnrollments SET deleted_at = NOW() WHERE enrollment_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);

        return Ok();
    }

}
