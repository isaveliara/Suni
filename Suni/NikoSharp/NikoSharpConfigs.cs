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

            foreach (var extClass in Configurations.ExternalClasses)
            {
                string fullTypeName = $"{extClass.CsAssembly}.{extClass.CsType}";
                Type type = Type.GetType(fullTypeName, throwOnError: false);
                if (type == null){
                    var assembly = Assembly.Load(extClass.CsAssembly);
                    type = assembly.GetType(fullTypeName);
                }
                if (type == null)
                    throw new Exception($"Type {fullTypeName} not found.");

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    // Opcional: filtrar apenas os métodos que você deseja expor
                    Delegate del = Delegate.CreateDelegate(typeof(Func<object[], object>), method);
                    // Cria uma chave, por exemplo: "Output::Add"
                    string key = $"{extClass.CsType}::{method.Name}";
                    Configurations.RegisteredFunctions[key] = del;
                }
            }
        }
        catch (Exception ex){
            Console.WriteLine($"Error: Cannot get .json config file. Message:\n{ex.Message}");
            throw;
        }
    }
}

public class NikosConfiguration //main class
{
    public List<ExternalClassEntry> ExternalClasses { get; set; } = new List<ExternalClassEntry>();
    public LanguageSettings LanguageSettings { get; set; } = new LanguageSettings();
    
    public Dictionary<string, Delegate> RegisteredFunctions { get; set; } = new Dictionary<string, Delegate>();
}

public class ExternalClassEntry
{
    public string CsAssembly { get; set; }
    public string CsType { get; set; }
}

public class LanguageSettings
{
    public int MaxIterations { get; set; }
}
