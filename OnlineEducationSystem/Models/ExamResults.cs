namespace OnlineEducationSystem.Models;

public class ExamResults
{
    public int result_id { get; set; }
    public int exam_id { get; set; }
    public int student_id { get; set; }
    public int score { get; set; } // Between 0 and 100
    public DateTime taken_at { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class CreateExamResults
{
    public int exam_id { get; set; }
    public int student_id { get; set; }
    public int score { get; set; } // Between 0 and 100
    public DateTime taken_at { get; set; }
}

public class PatchExamResults
{
    public int student_id { get; set; }
    public int exam_id { get; set; }
    public int score { get; set; } // Between 0 and 100
}
