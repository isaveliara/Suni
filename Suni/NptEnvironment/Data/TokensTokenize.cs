namespace Suni.Suni.NptEnvironment.Data;
partial class Tokens
{
    public static string[] Tokenize(string expression)
    {
        List<string> tokens = new List<string>();
        int len = expression.Length;
        int start = 0;
        int pos = 0;

        while (pos < len)
        {
            bool tokenFound = false;
            int tokenLength = 0;
            string token = null;

            //1. Check for { ... }. These tokens can be List or Dict or an expression.
            if (expression[pos] == '{'){
                int end = pos + 1;
                while (end < len && expression[end] != '}') end++;
                if (end < len)
                {
                    tokenLength = end - pos + 1;
                    token = expression.Substring(pos, tokenLength);
                    tokenFound = true;
                }
            }
            //1,5. Check for ( ... ). These tokens can be List or Dict or an expression.
            if (expression[pos] == '('){
                int end = pos + 1;
                while (end < len && expression[end] != ')') end++;
                if (end < len)
                {
                    tokenLength = end - pos + 1;
                    token = expression.Substring(pos, tokenLength);
                    tokenFound = true;
                }
            }

            if (!tokenFound)
                //2. Check for s'...'. These tokens are Literals (string).
                if (pos + 1 < len && expression[pos] == 's' && expression[pos + 1] == '\'')
                {
                    int end = pos + 2;
                    while (end < len && expression[end] != '\'') end++;
                    if (end < len)
                    {
                        tokenLength = end - pos + 1;
                        token = expression.Substring(pos, tokenLength);
                        tokenFound = true;
                    }
                }

            if (!tokenFound)
                //3. Check for c'...'. These tokens are Literals (char).
                if (pos + 3 < len &&
                    expression[pos] == 'c' &&
                    expression[pos + 1] == '\'' &&
                    expression[pos + 3] == '\'')
                {
                    tokenLength = 4;
                    token = expression.Substring(pos, tokenLength);
                    tokenFound = true;
                }

            if (!tokenFound)
                //4. Check for numbers. These tokens can be Int and Float.
                if (char.IsDigit(expression[pos])){
                    int end = pos;
                    while (end < len && char.IsDigit(expression[end])) end++;
                    if (end < len && expression[end] == ',')
                    {
                        int commaPos = end++;
                        while (end < len && char.IsDigit(expression[end])) end++;
                        if (end > commaPos + 1)
                        {
                            tokenLength = end - pos;
                            token = expression.Substring(pos, tokenLength);
                            tokenFound = true;
                        }
                    }
                }

            if (!tokenFound)
                //5. Check multi-character operators.
                if (pos + 1 < len)
                {
                    string twoChar = expression.Substring(pos, 2);
                    switch (twoChar)
                    {
                        case "||":
                        case "&&":
                        case "==":
                        case "~=":
                        case ">=":
                        case "<=":
                        case "::":
                            tokenLength = 2;
                            token = twoChar;
                            tokenFound = true;
                            break;
                    }
                }

            if (!tokenFound)
                //6. Check single-character operators/tokens.
                if (pos < len){
                    char c = expression[pos];
                    switch (c)
                    {
                        case '[':
                        case ']':
                        case '>':
                        case '<':
                        case '!':
                        case '?':
                        case '#':
                        case ',':
                        case '+':
                        case '-':
                        case '*':
                        case '/':
                            tokenLength = 1;
                            token = c.ToString();
                            tokenFound = true;
                            break;
                    }
                }

            if (!tokenFound)
                //7. Check whitespace token (discard).
                if (char.IsWhiteSpace(expression[pos])){
                    tokenLength = 1;
                    while (pos + tokenLength < len && 
                            char.IsWhiteSpace(expression[pos + tokenLength]))
                        tokenLength++;
                    
                    token = expression.Substring(pos, tokenLength);
                    tokenFound = true;
                }

            if (tokenFound){
                //Add non-token part
                if (pos > start){
                    string nonToken = expression.Substring(start, pos - start);
                    if (!string.IsNullOrWhiteSpace(nonToken))
                        tokens.Add(nonToken);
                }

                //Add token if not whitespace
                if (!string.IsNullOrWhiteSpace(token) && !IsWhitespaceToken(token))
                    tokens.Add(token);

                start = pos + tokenLength;
                pos = start;
            }
            else
                pos++;
        }

        //Add remaining non-token part
        if (start < len){
            string nonToken = expression.Substring(start);
            if (!string.IsNullOrWhiteSpace(nonToken))
                tokens.Add(nonToken);
        }
        tokens.Append("EOF");
        return [.. tokens];
    }

    private static bool IsWhitespaceToken(string token){
        foreach (char c in token)
            if (!char.IsWhiteSpace(c))
                return false;
        return true;
    }
}
