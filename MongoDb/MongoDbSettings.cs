namespace Api_Godot_PSP.MongoDb;

// Clase que define la configuración de MongoDB, como la cadena de conexión y el nombre de la base de datos
public class MongoDbSettings
{
    public string ConnectionString { get; init; } = "";
    public string DatabaseName { get; init; } = "";
}