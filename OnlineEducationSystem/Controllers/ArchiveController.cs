using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;
using Npgsql;
using Microsoft.AspNetCore.Authorization;

namespace OnlineEducationSystem.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ArchiveController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseHelper _dbHelper;

        public ArchiveController(IConfiguration configuration)
        {
            _configuration = configuration;
            var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
            _dbHelper = new DatabaseHelper(connectionString!);
        }

        [HttpGet("exams")]
        public IActionResult GetDeletedExams()
        {
            var query = "SELECT * FROM exams WHERE deleted_at IS NOT NULL";
            var exams = _dbHelper.ExecuteReader(query, reader => new Exams
            {
                exam_id = reader.GetInt32(0),
                course_id = reader.GetInt32(1),
                title = reader.GetString(2),
                description = reader.IsDBNull(3) ? null : reader.GetString(3),
                created_at = reader.GetDateTime(4),
                updated_at = reader.GetDateTime(5),
                deleted_at = reader.GetDateTime(6)
            }).ToList();

            return Ok(exams);
        }

        [HttpGet("courses")]
        public IActionResult GetDeletedCourses()
        {
            var query = "SELECT * FROM courses WHERE deleted_at IS NOT NULL";
            var courses = _dbHelper.ExecuteReader(query, reader => new Courses
            {
                course_id = reader.GetInt32(0),
                instructor_id = reader.GetInt32(1),
                title = reader.GetString(2),
                description = reader.IsDBNull(3) ? null : reader.GetString(3),
                created_at = reader.GetDateTime(4),
                updated_at = reader.GetDateTime(5),
                deleted_at = reader.GetDateTime(6)
            }).ToList();

            return Ok(courses);
        }

        [HttpGet("users")]
        public IActionResult GetDeletedUsers()
        {
            var query = "SELECT * FROM users WHERE deleted_at IS NOT NULL";
            var users = _dbHelper.ExecuteReader(query, reader => new Users
            {
                user_id = reader.GetInt32(0),
                email = reader.GetString(1),
                password = reader.GetString(2),
                role = reader.GetString(3),
                name = reader.GetString(4),
                created_at = reader.GetDateTime(5),
                updated_at = reader.GetDateTime(6),
                deleted_at = reader.GetDateTime(7)
            }).ToList();

            return Ok(users);
        }

        [HttpGet("certificates")]
        public IActionResult GetDeletedCertificates()
        {
            var query = "SELECT * FROM certificates WHERE deleted_at IS NOT NULL";
            var certificates = _dbHelper.ExecuteReader(query, reader => new Certificates
            {
                certificate_id = reader.GetInt32(0),
                course_id = reader.GetInt32(1),
                student_id = reader.GetInt32(2),
                issued_date = reader.GetDateTime(3),
                deleted_at = reader.GetDateTime(4)
            }).ToList();

            return Ok(certificates);
        }
    }
}
