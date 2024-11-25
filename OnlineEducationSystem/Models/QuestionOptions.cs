namespace OnlineEducationSystem.Models;

public class QuestionOptions
{
    public int option_id { get; set; }
    public int question_id { get; set; }
    public string option_text { get; set; } = string.Empty;
    public bool is_correct { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class CreateQuestionOptions
{
    public int question_id { get; set; }
    public string option_text { get; set; } = string.Empty;
    public bool is_correct { get; set; }
}

public class PatchQuestionOptions
{
    public int option_id { get; set; }
    public string option_text { get; set; } = string.Empty;
    public bool is_correct { get; set; }
}
