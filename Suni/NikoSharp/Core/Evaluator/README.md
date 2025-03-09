- notes


# Examples of Expressions:
```cs
// 1 + 1                       | 2 (i think is wrong.)
// true && false               | False
// s'hi' :: len == 2           | True
// a ? s'osvald'               | True
// a ? s'petter'               | False
// s'Hello Worl':: len         | HELLO WORLD
// s'zzzzzzzzzz':: len >= 10   | True
// s'hel' + s'lo' == s'hello'  | True
// s'a' + s'b' ? s'oiba'       | False
// s'a' + s'b' ? s'oiab'       | True
// 900 # s'hi'                 | Aleatory choices '900' or 'hi'
```

# Precedence of Tokens:
```cs
internal static int Precedence(string Operator)
{
    return Operator switch
    {
        "[" or "::" => 5,
        "!" => 4,
        "*" or "/" => 3,
        "+" or "-" => 2,
        "?" or "#" => 1,
        "&&" => 0,
        "||" => -1,
        _ => -2
    };
}
```

# Accessible types:
```cs
public enum STypes
{
    Nil, Bool, Int, Float,
    Str, Function, Char, List, Dict,
    Error, Identifier, //inaccessible when executing codes
}
```