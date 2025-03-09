using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api_Godot_PSP.Models;

// Modelo que representa a un usuario con nombre de usuario, contraseña (hash) y la puntuación más alta
public class User
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public int HighestScore { get; set; } = 0;
}
