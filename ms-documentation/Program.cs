using ms_documentation.Services;
using ms_documentation.Utils;
using StackExchange.Redis;

namespace ms_documentation;

public partial class Program {
    public static void Main(string[] args)
    {
        var env = new EnvironmentHandler();
        env.Load();
        IDatabase? db = null;
        ConnectionMultiplexer? connection = null;

        try
        {
            var config = new ConfigurationOptions
            {
                EndPoints = { env.Get("CACHE_ADDRESS") },
                Password = env.Get("CACHE_PASSWORD"),
                Ssl = false,
                AbortOnConnectFail = false
            };

            connection = ConnectionMultiplexer.Connect(config);

            if (connection.IsConnected)
                db = connection.GetDatabase();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fallo la conexión al cache: {ex.Message}");
            db = null;
        }
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<IAlumnoService>( 
            new AlumnoService(
                new Clients.AlumnosClient(new HttpClient { BaseAddress = new Uri(env.Get("ALUMNOS_API_URI")) }),
                new Clients.GestionClient(
                        new HttpClient { BaseAddress = new Uri(env.Get("GESTION_API_URI")) },
                        db
                )
            )
        );
        builder.Services.AddControllers();
        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
