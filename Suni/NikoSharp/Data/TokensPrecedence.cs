namespace Suni.Suni.NikoSharp.Data;

partial class Tokens
{
    internal static int Precedence(string Operator)
        {
            return Operator switch
            {
                "::" => 6,
                "[" or "]" or "{" or "}" => 5,
                "!" => 4,
                "*" or "/" => 3,
                "+" or "-" => 2,
                "?" or "#" => 1,
                "==" or "~=" or ">" or "<" or ">=" or "<=" => 0,
                "&&" => -1,
                "||" => -2,
                "," => -3,
                _ => -4
            };
        }
}