namespace OnlineEducationSystem.Models;

public class Courses
{
    public int student_id { get; set; }
    public int course_id { get; set; }
    public int instructor_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}
public class CreateCourses
{
    public int instructor_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; } = string.Empty;
}
public class PatchCourses
{
    public int course_id { get; set; }
    public int instructor_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; } = string.Empty;
}
