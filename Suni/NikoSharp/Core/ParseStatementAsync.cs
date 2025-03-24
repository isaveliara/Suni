using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    public async Task<Diagnostics> ParseStatementAsync()
    {
        if (_position >= _tokens.Length) return Diagnostics.Success;

        var current = CurrentToken();

        if (current == "EOL")
        {
            _position++;
            return await ParseStatementAsync();
        }

        if (PeekToken() == "::")
        {
            var result = await ParseMethodCallAsync();
            return result.Diagnostic;
        }

        if (IsType(current))
            return await ParseVariableDeclarationAsync();
        
        if (IsClass(current))
            return await ParseClassEditing();

        switch (current)
        {
            case "if": return await ParseIfStatementAsync();
            case "while": return await ParseWhileStatementAsync();
            case "for": return await ParseForStatementAsync();
            case "poeng": return await ParsePoengStatementAsync();
            case "exit": return ParseExitStatement();
        }

        throw new ParseException(Diagnostics.SyntaxException, $"UnexpectedToken: {current}");
    }
}