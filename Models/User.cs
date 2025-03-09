using MongoDB.Bson;

namespace Api_Godot_PSP.Models;

// Modelo que representa a un usuario con nombre de usuario, contraseña (hash) y la puntuación más alta
public class User
{
    public ObjectId? Id { get; set; } = null;
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public int HighestScore { get; set; } = 0;
}
