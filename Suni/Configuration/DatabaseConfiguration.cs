namespace Suni.Suni.Configuration;

public class DatabaseConfiguration
{
    public static void Configure()
    {
        var db = new DBMethods();
        db.Setup();
    }
}