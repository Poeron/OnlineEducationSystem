namespace OnlineEducationSystem.Models;

public class AssignmentSubmissions
{
    public int submission_id { get; set; }
    public int assignment_id { get; set; }
    public int student_id { get; set; }
    public string? submission_url { get; set; }
    public int? grade { get; set; } // Nullable for ungraded submissions
    public DateTime submitted_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class ViewAssignmentSubmission
{
    public string name { get; set; } = String.Empty;
    public int submission_id { get; set; }
    public int student_id { get; set; }
    public string? submission_url { get; set; }
    public int? grade { get; set; } // Nullable for ungraded submissions
    public DateTime submitted_at { get; set; }
}

public class SendSubmissions
{
    public int assignment_id { get; set; }
}

public class CreateAssignmentSubmissions
{
    public int assignment_id { get; set; }
    public int student_id { get; set; }
    public string? submission_url { get; set; }
    public DateTime? submitted_at { get; set; } = DateTime.Now;
}

public class UpdateAssignmentSubmissions
{
    public int submission_id { get; set; }
    public string? submission_url { get; set; }
    public int? grade { get; set; }
}

public class PatchAssignmentSubmissions
{
    public int submission_id { get; set; }
    public int grade { get; set; } // Between 0 and 100
}
