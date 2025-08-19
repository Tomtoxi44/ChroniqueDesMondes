using Cdm.Business.Common.Business.Users;
using Cdm.Business.Common.Business.Users.Models;
using Microsoft.AspNetCore.Mvc;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/register", async (UserRequest userRequest, UserBusiness userService, PasswordService passwordService) =>
        {
            userRequest.Password = passwordService.HashPassword(userRequest.Password);
            await userService.RegisterUserAsync(userRequest);
            return Results.Ok("Utilisateur enregistré avec succès !");
        });

        app.MapPost("/login", async ([FromBody] LoginRequest loginRequest, UserBusiness userService, PasswordService passwordService, JwtService jwtService) =>
        {
            var user = await userService.GetUserByEmailAsync(loginRequest.email);
            if (user == null || !passwordService.VerifyPassword(user.Password, loginRequest.password))
            {
                return Results.Unauthorized();
            }

            var token = jwtService.GenerateToken(user.Id.ToString(), user.UserEmail);
            return Results.Ok(new { Token = token });
        });
    }

    public sealed record LoginRequest(string email, string password);
}