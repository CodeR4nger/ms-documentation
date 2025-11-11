namespace ms_documentation.Utils;

public interface IEnvironmentHandler
{
    void Load();
    string Get(string key);
}
/// <summary>
/// Implementación de <see cref="IEnvironmentHandler"/> que utiliza DotNetEnv para cargar variables de entorno.
/// </summary>
public class EnvironmentHandler : IEnvironmentHandler
{
    private bool _isLoaded = false;
    /// <summary>
    /// Carga las variables de entorno desde un archivo .env. Este método es seguro para llamarse múltiples veces, pero solo realiza la carga una vez.
    /// </summary>
    public void Load()
    {
        if (!_isLoaded && !DotNetEnv.Env.GetBool("DOTNET_RUNNING_IN_CONTAINER"))
        {
            DotNetEnv.Env.Load();
            _isLoaded = true;
        }
    }
    /// <summary>
    /// Obtiene el valor de una variable de entorno. Lanza una excepción si la variable no está definida o está vacía.
    /// </summary>
    /// <param name="key">Clave de la variable de entorno.</param>
    /// <returns>El valor correspondiente a la clave especificada.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si la variable no está definida o está vacía.</exception>
    public string Get(string key)
    {
        var envValue = DotNetEnv.Env.GetString(key);
        if (string.IsNullOrEmpty(envValue))
            throw new InvalidOperationException($"Environment variable '{key}' is not set.");
        return envValue;
    }
}