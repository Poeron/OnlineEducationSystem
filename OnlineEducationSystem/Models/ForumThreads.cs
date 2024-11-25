namespace OnlineEducationSystem.Models;

public class ForumThreads
{
    public int thread_id { get; set; }
    public int course_id { get; set; }
    public int? author_id { get; set; } // Nullable in case the author is removed
    public string title { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? deleted_at { get; set; }
}

public class CreateForumThreads
{
    public int course_id { get; set; }
    public int? author_id { get; set; } // Optional
    public string title { get; set; } = string.Empty;
}

public class PatchForumThreads
{
    public int thread_id { get; set; }
    public string title { get; set; } = string.Empty;
}
