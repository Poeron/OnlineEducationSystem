namespace OnlineEducationSystem.Models;

public class Exams
{
    public int exam_id { get; set; }
    public int course_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class CreateExams
{
    public int course_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; }
}

public class PatchExams
{
    public int exam_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; }
}
