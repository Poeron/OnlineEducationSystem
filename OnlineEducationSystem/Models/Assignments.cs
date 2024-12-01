namespace OnlineEducationSystem.Models;

public class Assignments
{
    public int assignment_id { get; set; }
    public int course_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; }
    public DateTime? due_date { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class SendAssigments
{
    public string course_name { get; set; } = string.Empty;
    public int assignment_id { get; set; }
    public int course_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; }
    public DateTime? due_date { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class CreateAssignments
{
    public int course_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; }
    public DateTime? due_date { get; set; }
}

public class PatchAssignments
{
    public int assignment_id { get; set; }
    public string title { get; set; } = string.Empty;
    public string? description { get; set; }
    public DateTime? due_date { get; set; }
}
