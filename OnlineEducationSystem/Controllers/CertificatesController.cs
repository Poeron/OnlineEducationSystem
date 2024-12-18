using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CertificatesController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public CertificatesController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    [HttpGet]
    public IActionResult GetCertificates()
    {
        var query = "SELECT * FROM certificates";
        var certificates = _dbHelper.ExecuteReader(query, reader => new Certificates
        {
            certificate_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            issued_date = reader.GetDateTime(3),
            deleted_at = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
        }).Where(certificate => certificate.deleted_at == null).ToList();

        return Ok(certificates);
    }

    [HttpGet("student/{student_id}")]
    public IActionResult GetCertificateForStudent(int student_id)
    {
        var query = @"SELECT cer.certificate_id, c.title, u.name, i.name, cer.issued_date
            FROM certificates cer INNER JOIN courses c ON cer.course_id = c.course_id
            INNER JOIN users u ON cer.student_id = u.user_id
            INNER JOIN users i ON c.instructor_id = i.user_id
            WHERE cer.student_id = @student_id AND cer.deleted_at IS NULL";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@student_id",student_id)
        };

        var certificate = _dbHelper.ExecuteReader(query, reader => new ViewCertificate
        {
            certificate_id = reader.GetInt32(0),
            course_title = reader.GetString(1),
            student_name = reader.GetString(2),
            instructor_name = reader.GetString(3),
            issued_date = reader.GetDateTime(4)
        }, parameters).ToList();

        if (certificate == null)
        {
            return NotFound();
        }

        return Ok(certificate);
    }

    [HttpGet("{id}")]
    public IActionResult GetCertificate(int id)
    {
        var query = "SELECT * FROM certificates WHERE certificate_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var certificate = _dbHelper.ExecuteReader(query, reader => new Certificates
        {
            certificate_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            student_id = reader.GetInt32(2),
            issued_date = reader.GetDateTime(3),
            deleted_at = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
        }, parameters).FirstOrDefault();

        if (certificate == null || certificate.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(certificate);
    }

    [Authorize(Roles = "instructor,admin")]
    [HttpPost]
    public IActionResult CreateCertificate([FromBody] CreateCertificates certificate)
    {
        var query = "INSERT INTO certificates (course_id, student_id) VALUES (@course_id, @student_id)";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", certificate.course_id),
            new NpgsqlParameter("@student_id", certificate.student_id),
        };

        var certificateId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(certificateId);
    }

    [Authorize(Roles = "instructor,admin")]
    [HttpPatch]
    public IActionResult UpdateCertificate([FromBody] UpdateCertificates certificate)
    {
        var query = "UPDATE certificates SET course_id = @course_id, student_id = @student_id WHERE certificate_id = @certificate_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@certificate_id", certificate.certificate_id),
            new NpgsqlParameter("@course_id", certificate.course_id),
            new NpgsqlParameter("@student_id", certificate.student_id)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Kayıt Başarıyla Güncellendi." });
    }


    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteCertificate(int id)
    {
        var query = "UPDATE certificates SET deleted_at = @deleted_at WHERE certificate_id = @certificate_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@certificate_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Kayıt Başarıyla Silindi." });
    }
}
