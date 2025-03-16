namespace Suni.Suni.NikoSharp.NikoStandartLibrary;
public static class Output
{
    public static object Add(object[] args)
    {
        if (args == null || args.Length == 0)
            Console.WriteLine("Output.Add: no arguments provided.");
        else
            foreach (var arg in args)
                Console.WriteLine(arg?.ToString());
        return null;
    }
}

public static class Etc
{
    public static object Nothing(object[] args)
    {
        return null;
    }
}
