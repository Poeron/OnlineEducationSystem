using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEducationSystem.Helpers;
using Npgsql;
using OnlineEducationSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace OnlineEducationSystem.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DatabaseHelper _dbHelper;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
            var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
            _dbHelper = new DatabaseHelper(connectionString!);
        }

        [HttpGet("UserCount")]
        public IActionResult GetUserCount()
        {
            var query = "SELECT COUNT(*) FROM users WHERE deleted_at IS NULL";
            var userCount = _dbHelper.ExecuteReader(query, reader => reader.GetInt32(0)).FirstOrDefault();

            return Ok(userCount);
        }
        [HttpGet("CourseCount")]
        public IActionResult GetCourseCount()
        {
            var query = "SELECT COUNT(*) FROM courses WHERE deleted_at IS NULL";
            var courseCount = _dbHelper.ExecuteReader(query, reader => reader.GetInt32(0)).FirstOrDefault();

            return Ok(courseCount);
        }
        [HttpGet("ExamCount")]
        public IActionResult GetExamCount()
        {
            var query = "SELECT COUNT(*) FROM exams WHERE deleted_at IS NULL";
            var examCount = _dbHelper.ExecuteReader(query, reader => reader.GetInt32(0)).FirstOrDefault();

            return Ok(examCount);
        }
        [HttpGet("CertificateCount")]
        public IActionResult GetCertificateCount()
        {
            var query = "SELECT COUNT(*) FROM certificates WHERE deleted_at IS NULL";
            var certificateCount = _dbHelper.ExecuteReader(query, reader => reader.GetInt32(0)).FirstOrDefault();

            return Ok(certificateCount);
        }

        [HttpGet("RecentUsers")]
        public IActionResult GetRecentUsers()
        {
            var query = "SELECT * FROM users WHERE deleted_at IS NULL ORDER BY created_at DESC LIMIT 5";
            var recentUsers = _dbHelper.ExecuteReader(query, reader => new Users
            {
                user_id = reader.GetInt32(0),
                email = reader.GetString(1),
                password = reader.GetString(2),
                role = reader.GetString(3),
                name = reader.GetString(4),
                created_at = reader.GetDateTime(5),
                updated_at = reader.GetDateTime(6),
                deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            }).ToList();

            return Ok(recentUsers);

        }
    }
}
