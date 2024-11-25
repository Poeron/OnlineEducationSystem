namespace OnlineEducationSystem.Models;

public class CourseMaterials
{
    public int material_id { get; set; }
    public int course_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string content_type { get; set; } = string.Empty; // 'video', 'pdf', etc.
    public string content_url { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class CreateCourseMaterials
{
    public int course_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string content_type { get; set; } = string.Empty;
    public string content_url { get; set; } = string.Empty;
}

public class PatchCourseMaterials
{
    public int material_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string content_type { get; set; } = string.Empty;
    public string content_url { get; set; } = string.Empty;
}