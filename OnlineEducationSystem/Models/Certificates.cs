namespace OnlineEducationSystem.Models;

public class Certificates
{
    public int certificate_id { get; set; }
    public int course_id { get; set; }
    public int student_id { get; set; }
    public DateTime issued_date { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class ViewCertificate
{
    public int certificate_id { get; set; }
    public string course_title { get; set; } = String.Empty;
    public string student_name { get; set; } = String.Empty;
    public DateTime issued_date { get; set; }
}
public class CreateCertificates
{
    public int course_id { get; set; }
    public int student_id { get; set; }
}
public class UpdateCertificates
{

   public int certificate_id { get; set; }
    public int course_id { get; set; }
    public int student_id { get; set; }
    public DateTime issued_date { get; set; }
}
