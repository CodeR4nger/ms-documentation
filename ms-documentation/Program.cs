using ms_documentation.Services;
using ms_documentation.Utils;

namespace ms_documentation;

public partial class Program {
    public static void Main(string[] args)
    {
        var env = new EnvironmentHandler();
        env.Load();
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<IAlumnoService>( new AlumnoService(
            new Clients.AlumnosClient(new HttpClient { BaseAddress = new Uri(env.Get("ALUMNOS_API_URI")) }),
            new Clients.GestionClient(new HttpClient { BaseAddress = new Uri(env.Get("GESTION_API_URI")) })
            )
        );
        builder.Services.AddControllers();
        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
