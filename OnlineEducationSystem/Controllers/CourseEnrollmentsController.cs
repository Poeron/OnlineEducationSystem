﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Authorize]
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

    [HttpGet]
    public IActionResult GetCourseEnrollments()
    {
        var query = "SELECT * FROM CourseEnrollments";
        var enrollments = _dbHelper.ExecuteReader(query, reader => new CourseEnrollments
        {
            enrollment_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            enrollment_date = reader.GetDateTime(3),
            deleted_at = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
        }).Where(enrollment => enrollment.deleted_at == null).ToList();

        return Ok(enrollments);
    }

    [HttpGet("{id}")]
    public IActionResult GetCourseEnrollment(int id)
    {
        var query = "SELECT * FROM CourseEnrollments WHERE enrollment_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var enrollment = _dbHelper.ExecuteReader(query, reader => new CourseEnrollments
        {
            enrollment_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            enrollment_date = reader.GetDateTime(3),
            deleted_at = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
        }, parameters).FirstOrDefault();

        if (enrollment == null || enrollment.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(enrollment);
    }

    [Authorize(Roles = "student, admin")]
    [HttpPost]
    public IActionResult CreateCourseEnrollment([FromBody] CreateCourseEnrollments enrollment)
    {
        var query = "INSERT INTO CourseEnrollments (course_id, student_id) VALUES (@course_id, @student_id)";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", enrollment.course_id),
            new NpgsqlParameter("@student_id", enrollment.student_id)
        };

        var enrollmentId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(enrollmentId);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult DeleteCourseEnrollment(int id)
    {
        var query = "UPDATE CourseEnrollments SET deleted_at = @deleted_at WHERE enrollment_id = @enrollment_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@enrollment_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Kayıt Başarıyla Silindi." });
    }
    [Authorize(Roles="student")]
    [HttpDelete("course/{courseId}")]
    public IActionResult DeleteCourseEnrollmentByCourseId(int courseId)
    {
        // get student id from token
        var studentId = int.Parse(User.Claims.First(claim => claim.Type == "user_id").Value);
        var query = "UPDATE CourseEnrollments SET deleted_at = @deleted_at WHERE course_id = @course_id AND student_id = @student_id AND deleted_at IS NULL";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", courseId),
            new NpgsqlParameter("@student_id", studentId),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Kayıt Başarıyla Silindi." });
    }
}
