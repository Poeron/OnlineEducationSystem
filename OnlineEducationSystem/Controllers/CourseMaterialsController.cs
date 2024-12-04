using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using OnlineEducationSystem.Helpers;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseMaterialsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly DatabaseHelper _dbHelper;

    public CourseMaterialsController(IConfiguration configuration)
    {
        _configuration = configuration;
        var connectionString = configuration.GetConnectionString("PostgreSqlConnection");
        _dbHelper = new DatabaseHelper(connectionString!);
    }

    [HttpGet]
    public IActionResult GetCourseMaterials()
    {
        var query = "SELECT * FROM CourseMaterials";
        var materials = _dbHelper.ExecuteReader(query, reader => new CourseMaterials
        {
            material_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            title = reader.GetString(2),
            content_type = reader.GetString(3),
            content_url = reader.GetString(4),
            created_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }).Where(material => material.deleted_at == null).ToList();

        return Ok(materials);
    }

    [HttpGet("Course/{id}")]
    public IActionResult GetCourseMaterialsByCourse(int id)
    {
        var query = "SELECT * FROM CourseMaterials WHERE course_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var materials = _dbHelper.ExecuteReader(query, reader => new CourseMaterials
        {
            material_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            title = reader.GetString(2),
            content_type = reader.GetString(3),
            content_url = reader.GetString(4),
            created_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }, parameters).Where(material => material.deleted_at == null).ToList();

        return Ok(materials);
    }

    [HttpGet("{id}")]
    public IActionResult GetCourseMaterial(int id)
    {
        var query = "SELECT * FROM CourseMaterials WHERE material_id = @id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@id", id)
        };

        var material = _dbHelper.ExecuteReader(query, reader => new CourseMaterials
        {
            material_id = reader.GetInt32(0),
            course_id = reader.GetInt32(1),
            title = reader.GetString(2),
            content_type = reader.GetString(3),
            content_url = reader.GetString(4),
            created_at = reader.GetDateTime(5),
            updated_at = reader.GetDateTime(6),
            deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
        }, parameters).FirstOrDefault();

        if (material == null || material.deleted_at != null)
        {
            return NotFound();
        }

        return Ok(material);
    }

    [Authorize(Roles = "instructor, admin")]
    [HttpPost]
    public IActionResult CreateCourseMaterial([FromBody] CreateCourseMaterials material)
    {
        var query = "INSERT INTO CourseMaterials (course_id, title, content_type, content_url) VALUES (@course_id, @title, @content_type, @content_url)";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@course_id", material.course_id),
            new NpgsqlParameter("@title", material.title),
            new NpgsqlParameter("@content_type", material.content_type),
            new NpgsqlParameter("@content_url", material.content_url)
        };

        var materialId = _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(materialId);
    }

    [Authorize(Roles = "instructor, admin")]
    [HttpPatch]
    public IActionResult UpdateCourseMaterial([FromBody] PatchCourseMaterials material)
    {
        var query = "UPDATE CourseMaterials SET title = @title, content_type = @content_type, content_url = @content_url WHERE material_id = @material_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@material_id", material.material_id),
            new NpgsqlParameter("@title", material.title),
            new NpgsqlParameter("@content_type", material.content_type),
            new NpgsqlParameter("@content_url", material.content_url)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Güncelleme Başarılı" });
    }

    [Authorize(Roles = "instructor,admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteCourseMaterial(int id)
    {
        var query = "UPDATE CourseMaterials SET deleted_at = @deleted_at WHERE material_id = @material_id";
        var parameters = new NpgsqlParameter[]
        {
            new NpgsqlParameter("@material_id", id),
            new NpgsqlParameter("@deleted_at", DateTime.Now)
        };

        _dbHelper.ExecuteNonQuery(query, parameters);
        return Ok(new { message = "Kayıt Başarıyla Silindi." });
    }
}
