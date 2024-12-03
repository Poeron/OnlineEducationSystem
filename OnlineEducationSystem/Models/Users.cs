namespace OnlineEducationSystem.Models;

public class Users
{
    public int user_id { get; set; }
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public DateTime created_at { get; set; } 
    public DateTime updated_at { get; set; } 
    public DateTime? deleted_at { get; set; } 
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

public class CreateUser
{
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty; // Passwords should be hashed
    public string role { get; set; } = "student"; // Default role is student
    public string name { get; set; } = string.Empty;
}

public class UpdateUser
{
    public int user_id { get; set; }
    public string name { get; set; } = string.Empty;
    public string role { get; set; } = "student"; // Optional role change
}

public class Students
{
    public int user_id { get; set; }
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public DateTime enrollment_date { get; set; }
    public int enrollment_id { get; set; }
}