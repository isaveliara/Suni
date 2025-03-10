using Suni.Suni.NikoSharp.Data;
namespace Suni.Suni.NikoSharp.Core;

public partial class NikoSharpParser{

    /// <summary>
    /// Gets the tokens from a block.
    /// Assumes that the "do" token has already been consumed.
    /// </summary>
    private List<string> CaptureBlockTokens(int initialDepth = 0)
    {
        List<string> blockTokens = new List<string>();
        int depth = initialDepth;
        
        while (_position < _tokens.Length)
        {
            string token = _tokens[_position];
            if (token == "do"){
                depth++;
                blockTokens.Add(ConsumeToken());
            }
            else if (token == "end")
            {
                if (depth == initialDepth){
                    ConsumeToken("end");
                    return blockTokens;
                }
                else{
                    depth--;
                    blockTokens.Add(ConsumeToken());
                }
            }
            else
                blockTokens.Add(ConsumeToken());
        }
        throw new ParseException(Diagnostics.SyntaxException, "Expected 'end' Token.");
    }

    /// <summary>
    /// Executes a block of code from a copy of its tokens.
    /// </summary>
    private async Task<Diagnostics> ExecuteBlockAsync_Internal(List<string> blockTokens)
    {
        var blockParser = new NikoSharpParser(blockTokens.ToArray(), _context);
        while (blockParser.CurrentToken() != "EOF")
        {
            Diagnostics result = await blockParser.ParseStatementAsync();
            if (result != Diagnostics.Success)
                return result;
        }
        return Diagnostics.Success;
    }

    /// <summary>
    /// Centralized version of ExecuteBlockAsync.
    /// </summary>
    private async Task<Diagnostics> ExecuteBlockAsync()
    {
        List<string> blockTokens = CaptureBlockTokens();
        return await ExecuteBlockAsync_Internal(blockTokens);
    }
}