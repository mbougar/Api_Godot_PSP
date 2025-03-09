using System.Security.Cryptography;
using Api_Godot_PSP.Models;
using Api_Godot_PSP.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api_Godot_PSP.Controllers;

// Controlador que maneja las peticiones relacionadas con los usuarios, como registro, inicio de sesión y envío de puntuaciones
[ApiController]
[Route("api/user")]
public class UserController(MongoDbService mongoDbService) : ControllerBase
{
    // Registro de usuario
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        var hashedPassword = HashPassword(user.PasswordHash);
        user.PasswordHash = hashedPassword;

        var success = await mongoDbService.CreateUser(user);
        if (!success) return BadRequest("Username already exists");

        return Ok("User registered successfully");
    }

    // Inicio de sesión de usuario
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User user)
    {
        var existingUser = await mongoDbService.GetUserAsync(user.Username);
        if (existingUser == null || !VerifyPassword(user.PasswordHash, existingUser.PasswordHash))
            return Unauthorized("Invalid credentials");

        return Ok("Login successful");
    }

    // Enviar la puntuación más alta
    [HttpPost("submit-score")]
    public async Task<IActionResult> SubmitScore([FromBody] User user)
    {
        var success = await mongoDbService.UpdateScore(user.Username, user.HighestScore);

        return Ok(!success ? "Score not updated (either user not found or score is not higher)" : "Score updated successfully");
    }

    // Obtener el ranking de los usuarios
    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard()
    {
        var leaderboard = await mongoDbService.GetLeaderboard();
        return Ok(leaderboard);
    }

    // Método para generar el hash de la contraseña (PBKDF2)
    private static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        return Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
    }

    // Método para verificar la contraseña comparando el hash almacenado
    private static bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHashBytes = Convert.FromBase64String(parts[1]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var computedHash = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
        }
        catch
        {
            return false;
        }
    }
}
