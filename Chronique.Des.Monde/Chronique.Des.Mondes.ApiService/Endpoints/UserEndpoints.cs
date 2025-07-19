using Chronique.Des.Mondes.User.Model;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/register", async (UserRequest userRequest, UserService userService, PasswordService passwordService) =>
        {
            userRequest.Password = passwordService.HashPassword(userRequest.Password);
            await userService.RegisterUserAsync(userRequest);
            return Results.Ok("Utilisateur enregistré avec succès !");
        });

        app.MapPost("/login", async (string email, string password, UserService userService, PasswordService passwordService, JwtService jwtService) =>
        {
            var user = await userService.GetUserByEmailAsync(email);
            if (user == null || !passwordService.VerifyPassword(user.Password, password))
            {
                return Results.Unauthorized();
            }

            var token = jwtService.GenerateToken(user.Id.ToString(), user.UserEmail);
            return Results.Ok(new { Token = token });
        });
    }
}