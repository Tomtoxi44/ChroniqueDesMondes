namespace Cdm.Web.Models;

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResponse
{
    public string Token { get; set; } = "";
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}

public class RegisterRequest
{
    public string UserName { get; set; } = "";
    public string UserEmail { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = "";
    public List<string> Errors { get; set; } = new();
}

public class ThemeInfo
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Description { get; set; } = "";
}