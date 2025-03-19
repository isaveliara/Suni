namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    public static class Std //TODO: Allow creating an instance
    {
        public static object Out(object[] args)
        {
            if (args != null || args.Length > 0)
                foreach (var arg in args)
                    Console.WriteLine("nikosharp -- "+arg?.ToString());
            return null;
        }
    }
}
