using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEducationSystem.Helpers;
using Npgsql;
using OnlineEducationSystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace OnlineEducationSystem.Controllers
{
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
                course_id = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                title = reader.GetString(2),
                description = reader.IsDBNull(3) ? null : reader.GetString(3),
                due_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                created_at = reader.GetDateTime(5),
                updated_at = reader.GetDateTime(6),
                deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            });
            // remove deleted assignments
            assignments = assignments.Where(assignment => assignment.deleted_at == null).ToList();
            
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
                course_id = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                title = reader.GetString(2),
                description = reader.IsDBNull(3) ? null : reader.GetString(3),
                due_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                created_at = reader.GetDateTime(5),
                updated_at = reader.GetDateTime(6),
                deleted_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
            }, parameters).FirstOrDefault();

            // check if assignment is deleted or there is no assignment with the given id
            if (assignment == null || assignment.deleted_at != null)
            {
                return NotFound();
            }
            return Ok(assignment);
        }

        [HttpPost]
        public IActionResult CreateAssignment([FromBody] CreateAssignments assignment)
        {
            var query = "INSERT INTO assignments (course_id, title, description, due_date) VALUES (@course_id, @title, @description, @due_date)";
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@course_id", assignment.course_id),
                new NpgsqlParameter("@title", assignment.title),
                new NpgsqlParameter("@description", assignment.description),
                new NpgsqlParameter("@due_date", assignment.due_date)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);

            return Ok();
        }
        [HttpPatch]
        public IActionResult UpdateAssignment([FromBody] PatchAssignments assignment)
        {
            var query = "UPDATE assignments SET course_id = @course_id, title = @title, description = @description, due_date = @due_date WHERE assignment_id = @assignment_id";
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@course_id", assignment.course_id),
                new NpgsqlParameter("@title", assignment.title),
                new NpgsqlParameter("@description", assignment.description),
                new NpgsqlParameter("@due_date", assignment.due_date),
                new NpgsqlParameter("@assignment_id", assignment.assignment_id)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteAssignment(int id)
        {
            var query = "UPDATE assignments SET deleted_at = @deleted_at WHERE assignment_id = @assignment_id";
            var parameters = new NpgsqlParameter[]
            {
                new NpgsqlParameter("@deleted_at", DateTime.Now),
                new NpgsqlParameter("@assignment_id", id)
            };

            _dbHelper.ExecuteNonQuery(query, parameters);

            return Ok();
        }
    }
}
