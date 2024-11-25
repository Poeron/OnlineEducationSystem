namespace OnlineEducationSystem.Models;

public class Certificates
{
    public int certificate_id { get; set; }
    public int course_id { get; set; }
    public int student_id { get; set; }
    public DateTime issued_date { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class CreateCertificates
{
    public int course_id { get; set; }
    public int student_id { get; set; }
    public DateTime issued_date { get; set; } = DateTime.Now; // Default to current date
}
