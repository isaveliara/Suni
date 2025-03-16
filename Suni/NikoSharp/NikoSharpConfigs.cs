using System.IO;
using System.Reflection;
using System.Text.Json;
namespace Suni.Suni.NikoSharp;

public static class NikoSharpConfigs
{
    public static NikosConfiguration Configurations { get; private set; }

    public static void SetupEnvironment(string jsonFilePath = "nikos.json")
    {
        try
        {
            string jsonConfig = File.ReadAllText(jsonFilePath);
            Configurations = JsonSerializer.Deserialize<NikosConfiguration>(jsonConfig);
            if (Configurations == null)
                throw new Exception("Configuração retornou null.");

            foreach (var func in Configurations.ExternalFunctions)
            {
                string fullTypeName = $"{func.CsAssembly}.{func.CsType}";
                Type type = Type.GetType(fullTypeName, throwOnError: false);
                if (type == null)
                {
                    var assembly = Assembly.Load(func.CsAssembly);
                    type = assembly.GetType(fullTypeName);
                }
                if (type == null)
                    throw new Exception($"Type {fullTypeName} not found.");

                var method = type.GetMethod(func.CsMethod, BindingFlags.Public | BindingFlags.Static);
                if (method == null)
                    throw new Exception($"Method {func.CsMethod} not found in type {fullTypeName}.");

                Delegate del = Delegate.CreateDelegate(typeof(Func<object[], object>), method);
                Configurations.RegisteredFunctions[func.Key] = del;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: Cannot get .nikosproj config file. Message:\n{ex.Message}");
            throw;
        }
    }
}

public class NikosConfiguration //main class
{
    public List<ExternalFunctionEntry> ExternalFunctions { get; set; } = new List<ExternalFunctionEntry>();
    public LanguageSettings LanguageSettings { get; set; } = new LanguageSettings();
    
    public Dictionary<string, Delegate> RegisteredFunctions { get; set; } = new Dictionary<string, Delegate>();
}

public class ExternalFunctionEntry
{
    public string Key { get; set; }
    public string CsAssembly { get; set; }
    public string CsType { get; set; }
    public string CsMethod { get; set; }
}

public class LanguageSettings
{
    public int MaxIterations { get; set; }
}
