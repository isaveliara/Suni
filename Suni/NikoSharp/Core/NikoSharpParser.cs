using Suni.Suni.NikoSharp.Data;
using Suni.Suni.NikoSharp.Data.Types;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser
{
    private readonly string[] _tokens;
    private int _position;
    private readonly EnvironmentDataContext _context;

    public NikoSharpParser(string[] tokens, EnvironmentDataContext context)
    {
        _tokens = tokens;
        _context = context;
        _position = 0;

        _context.Debugs.Add($"all tokens: {string.Join(" % ", _tokens)}");//debugging///////////
    }
    
    private string ConsumeToken(string expected = null)
    {
        while (_position < _tokens.Length && _tokens[_position] == "EOL")
            _position++;
        if (_position >= _tokens.Length || _tokens[_position] == "EOF")
            throw new ParseException(Diagnostics.SyntaxException, "Unexpected End of FIle.");
        
        var currentToken = CurrentToken();
        if (expected != null && currentToken != expected)
            throw new ParseException(Diagnostics.SyntaxException, $"Expected: '{expected}', Got: '{currentToken}'");

        return _tokens[_position++];
    }

    public string CurrentToken()
    {
        while (_position < _tokens.Length && _tokens[_position] == "EOL")
            _position++;

        return _position < _tokens.Length ? _tokens[_position] : "EOF";
    }

    private string PeekToken(int aditionalPos = 1)
    {
        int peekPos = _position + aditionalPos;
        while (peekPos < _tokens.Length && _tokens[peekPos] == "EOL")
            peekPos++;
        return peekPos < _tokens.Length ? _tokens[peekPos] : "EOF";
    }

    private bool IsType(string token) => Enum.TryParse<STypes>(token, out _);

    private bool IsClass(string token)
    {
        if (_context.BlockStack.Peek().LocalVariables.ContainsKey(token))
            if (_context.BlockStack.Peek().LocalVariables[token].Type == STypes.Class)
                return true;
        return false;
    }
}
