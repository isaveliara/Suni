# Npt

Npt is an interpreter, inspired by C++, C#, and Lua (a bit).

It is a language designed to interact with Discord's API, featuring dynamic and strong typing, and a simple syntax. At first glance, you can notice a few key characteristics:

- **Definitions Statement**: Requires a Definitions block at the top of the file to set environment rules and values.
- **Predefined Classes**: Comes with predefined classes (with methods), which differ from functions added by the code itself. The syntax for class methods is `ClassName::Method(args) -> Pointer .`. In the Definitions block, you can use `~include` to simplify usage to just `Method(args) -> Pointer`.
- **Unimplemented Features**: Some features are not yet implemented, such as loops, `else` statements, assigning `List`, `Dict`, and `Function` types to variables, etc.

---

# Syntax Examples

## Hello World
```npt
std::nout() -> s'Hello World'
```

## Definitions Block and Types
```npt
#definitions .

-- Variables and types
~Int set Variable1 90000 .
~Str set Variable2 s'Hello World' .
~Char set Variable3 c'A' .
~Bool set Variable4 true .
~Float set Variable5 3.14 .
~Nil set Variable6 nil . -- I don't know why you'd use this.
~Int set Variable7 800 - 400 .

-- Includes
~include npt . -- The only class, damn.

#ends .
```

## If Statement
```npt
&if (true || false) &do{ .
    std::nout() -> s'Hello World' .
    &if (1+1 == 2) &do{ .
        std::nout() -> s'1+1 = 2' .
    &} .
&} .
```
