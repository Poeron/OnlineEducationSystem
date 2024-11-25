using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

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
        var query = "INSERT INTO certificates (course_id, student_id, issued_date) VALUES (@course_id, @student_id, @issued_date)";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", certificate.course_id),
            new NpgsqlParameter("@student_id", certificate.student_id),
            new NpgsqlParameter("@issued_date", certificate.issued_date)
        };

        var certificateId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(certificateId);
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
        return Ok();
    }
}
