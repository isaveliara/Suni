namespace Suni.Suni.NptEnvironment.Data;
partial class Tokens
{
    internal static int Precedence(string Operator)
        {
            return Operator switch
            {
                "[" => 5,
                "!" => 4,
                "*" or "/" => 3,
                "+" or "-" => 2,
                "?" or "#" => 1,
                "&&" => 0,
                "||" => -1,
                _ => -2
            };
        }
}