namespace OnlineEducationSystem.Models;

public class User
{
    public int user_id { get; set; }
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public DateTime created_at { get; set; } 
    public DateTime updated_at { get; set; } 
    public DateTime deleted_at { get; set; } 
}

public class LoginDTO
{
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}

public class RegisterDTO
{
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
}