namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    private string ParseEncapsulation(char open, char close) => ConsumeToken().Trim(open, close);
}