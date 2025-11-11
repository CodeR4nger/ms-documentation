using ms_documentation.Utils;

namespace ms_documentation;

public partial class Program {
    public static void Main(string[] args)
    {
        var env = new EnvironmentHandler();
        env.Load();
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
