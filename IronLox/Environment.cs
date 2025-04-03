namespace IronLox;

public class Environment
{
    private readonly Dictionary<string, object?> _values = [];

    public void Define(string name, object? value)
    {
        // per Bob, always update if already exists..
        _values[name] = value;
    }

    public object? Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }

        throw new RuntimeError(name, $"Undefined variable {name.Lexeme}.");
    }
}