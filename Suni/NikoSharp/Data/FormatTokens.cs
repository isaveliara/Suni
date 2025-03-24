namespace Suni.Suni.NikoSharp.Data;

partial class Tokens
{
    public static string FormatTokens(string[] tokens)
    {
        var formatted = new List<string>();
        var currentLine = new List<string>();

        foreach (var token in tokens)
        {
            if (token == "EOL")
            {
                formatted.Add(string.Join(" ", currentLine));
                currentLine.Clear();
            }
            else
                currentLine.Add(token);
        }

        if (currentLine.Count > 0)
            formatted.Add(string.Join(" ", currentLine));

        return string.Join("\n", formatted);
    }
}
