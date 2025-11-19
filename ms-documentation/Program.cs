using ms_documentation.Policies;

using ms_documentation.Services;
using ms_documentation.Utils;
using StackExchange.Redis;

namespace ms_documentation;

public partial class Program {
    public static void Main(string[] args)
    {
        var env = new EnvironmentHandler();
        env.Load();
        ConnectionMultiplexer? connection = null;
        var builder = WebApplication.CreateBuilder(args);
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
            {
                Console.WriteLine("Conexion al cache establecida");
                builder.Services.AddSingleton(connection.GetDatabase());
            }
            else
            {
                Console.WriteLine("La conexion al cache fallo, no se configurara el cache.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fallo la conexion al cache: {ex.Message}");
        }
        
       builder.Services.AddHttpClient<Clients.IClienteAlumnos, Clients.AlumnosClient>(client =>
        {
            client.BaseAddress = new Uri(env.Get("ALUMNOS_API_URI"));
        }).AddPolicyHandler(PollyPolicies.GetResiliencePolicy());
        builder.Services.AddHttpClient<Clients.IClienteGestion,Clients.GestionClient>(client =>
        {
            client.BaseAddress = new Uri(env.Get("GESTION_API_URI"));
        })
        .AddPolicyHandler(PollyPolicies.GetResiliencePolicy());
        builder.Services.AddSingleton<IAlumnoService, AlumnoService>();
        builder.Services.AddControllers();
        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
