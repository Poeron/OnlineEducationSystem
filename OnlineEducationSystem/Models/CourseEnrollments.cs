namespace OnlineEducationSystem.Models;

public class CourseEnrollments
{
    public int enrollment_id { get; set; }
    public int course_id { get; set; }
    public int student_id { get; set; }
    public DateTime enrollment_date{ get; set; }
    public DateTime? deleted_at { get; set; }
}

public class CreateCourseEnrollments
{
    public int course_id { get; set; }
}
