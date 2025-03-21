namespace Suni.Suni.NikoSharp;

public static class Std
{
    public static object Out(object[] args)
    {
        if (args != null || args.Length > 0)
            foreach (var arg in args)
                Console.WriteLine("nikosharp out -- "+arg?.ToString());
        return null;
    }
}
