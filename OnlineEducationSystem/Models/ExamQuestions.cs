namespace OnlineEducationSystem.Models;

public class ExamQuestions
{
    public int question_id { get; set; }
    public int exam_id { get; set; }
    public string question_text { get; set; } = string.Empty;
    public string question_type { get; set; } = string.Empty; // 'multiple_choice', 'true_false', 'open_ended'
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class CreateExamQuestions
{
    public int exam_id { get; set; }
    public string question_text { get; set; } = string.Empty;
    public string question_type { get; set; } = string.Empty; // 'multiple_choice', 'true_false', 'open_ended'
}

public class PatchExamQuestions
{
    public int question_id { get; set; }
    public string question_text { get; set; } = string.Empty;
    public string question_type { get; set; } = string.Empty; // 'multiple_choice', 'true_false', 'open_ended'
}
