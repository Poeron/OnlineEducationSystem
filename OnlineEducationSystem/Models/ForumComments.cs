namespace OnlineEducationSystem.Models;

public class ForumComments
{
    public int comment_id { get; set; }
    public int thread_id { get; set; }
    public int? author_id { get; set; } // Nullable in case the author is removed
    public string comment_text { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class CreateForumComments
{
    public int course_id { get; set; }
    public int author_id { get; set; }
    public string comment_text { get; set; } = string.Empty;
}

public class PatchForumComments
{
    public int comment_id { get; set; }
    public string comment_text { get; set; } = string.Empty;
}
