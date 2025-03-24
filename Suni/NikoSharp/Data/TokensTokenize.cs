namespace Suni.Suni.NikoSharp.Data;

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

            //1. Check for commentary
            if (pos + 1 < len && expression[pos] == '-' && expression[pos + 1] == '-')
            {
                pos += 2;

                if (pos + 1 < len && expression[pos] == '[' && expression[pos + 1] == '['){
                    pos += 2;
                    while (pos + 1 < len && !(expression[pos] == ']' && expression[pos + 1] == ']'))
                        pos++;
                    if (pos + 1 < len && expression[pos] == ']' && expression[pos + 1] == ']')
                        pos += 2;
                }
                else
                    while (pos < len && expression[pos] != '\n' && expression[pos] != '.')
                        pos++;
                
                start = pos;
                continue;
            }

            //2. Check for EOL tokens.
            if (expression[pos] == '.' || expression[pos] == '\n')
            {
                if (pos > start)
                {
                    string nonToken = expression.Substring(start, pos - start);
                    if (!string.IsNullOrWhiteSpace(nonToken))
                        tokens.Add(nonToken);
                }
                tokens.Add("EOL");
                pos++;
                start = pos;
                continue;
            }

            //3. Check for { ... }. These tokens can be List or Dict or an expression.
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
            //4. Check for ( ... ). These tokens can be List or Dict or an expression.
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
                //5. Check for '...'. These tokens are Literals (string).
                if (pos + 1 < len && expression[pos] == '\'')
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
                //7. Check for numbers. These tokens can be Int and Float.
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
                //8. Check multi-character operators.
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
                //9. Check single-character operators/tokens.
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
                //10. Check whitespace token (discard).
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
