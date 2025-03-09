using Api_Godot_PSP.Models;
using Api_Godot_PSP.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api_Godot_PSP.Services;

// Servicio que interactúa con MongoDB para gestionar los usuarios y sus puntuaciones más altas
public class MongoDbService
{
    private readonly IMongoCollection<User> _users;

    public MongoDbService(IOptions<MongoDbSettings> settings)
    {
        // Establece la conexión con la base de datos MongoDB usando la configuración proporcionada
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _users = database.GetCollection<User>("Users"); // Nombre de la coleccion dentro de nuestra base de datos

        // Asegura que los nombres de usuario sean únicos al crear un índice en el campo 'Username'
        var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.Username);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<User>(indexKeys, indexOptions);
        _users.Indexes.CreateOne(indexModel);
    }

    // Recupera un usuario de la base de datos por nombre de usuario
    public async Task<User?> GetUser(string username) =>
        await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

    // Crea un nuevo usuario en la base de datos
    public async Task<bool> CreateUser(User user)
    {
        try
        {
            await _users.InsertOneAsync(user);
            return true;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return false; // El nombre de usuario ya existe
        }
    }

    // Actualiza la puntuación más alta de un usuario
    public async Task<bool> UpdateScore(string username, int newScore)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Username, username);
        var existingUser = await _users.Find(filter).FirstOrDefaultAsync();

        if (existingUser == null || newScore <= existingUser.HighestScore)
            return false; // No es necesario actualizar

        var update = Builders<User>.Update.Set(u => u.HighestScore, newScore);
        await _users.UpdateOneAsync(filter, update);
        return true;
    }

    // Recupera el ranking de los usuarios ordenados por puntuación más alta
    public async Task<List<User>> GetLeaderboard() =>
        await _users.Find(_ => true).SortByDescending(user => user.HighestScore).ToListAsync();
}
