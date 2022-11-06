namespace IronLox;
using static TokenType; 
public sealed class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens = new List<Token>();

    private int start = 0;
    private int current = 0;
    private int line = 1;
    public Scanner(string source)
    {
        this.source = source;
    }

    internal List<Token> ScanTokens()
    {
        while (NotAtEnd)
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new(EOF, line, "", null));
        return tokens;
    }

    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            case '(': AddToken(LEFT_PAREN);   break;
            case ')': AddToken(RIGHT_PAREN);  break;
            case '{': AddToken(LEFT_BRACE);   break;
            case '}': AddToken(RIGHT_BRACE);  break;
            case ',': AddToken(COMMA);        break;
            case '.': AddToken(DOT);          break;
            case '-': AddToken(MINUS);        break;
            case '+': AddToken(PLUS);         break;
            case ';': AddToken(SEMICOLON);    break;
            case '*': AddToken(STAR);         break;

            case '!':
                AddToken(Match('=') ? BANG_EQUAL : BANG); 
                break;
            case '=':
                AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? LESS_EQUAL : LESS);
                break;
            case '>':
                AddToken(Match('=') ? GREATER_EQUAL : GREATER);
                break;

            case '/':
                if(Match('/'))
                {
                    while (Peek() != '\n' && NotAtEnd) Advance();
                }
                else
                {
                    AddToken(SLASH);
                }
                break;

            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace
                break;

            case '\n':
                line++;
                break;

            default:
                //TODO: "Coalescing a run of invalid characters into a single error would give a nicer user experience" -Bob
                Lox.Error(line, $"Unexpexted character: {c}");
                break;
        }


    }

    private char Advance() => source[current++];
    

    private void AddToken(TokenType type, object? literal = null)
    {
        string text = source.Substring(start, current);
        tokens.Add(new(type, line, text, literal));
    }

    private bool Match(char expected)
    {
        if(IsAtEnd) return false;

        if(source[current] != expected) return false;

        current++;
        return true;
    }

    private char Peek()
    {
        if (IsAtEnd) return '\0';

        return source[current];
    }

    private bool IsAtEnd => current >= source.Length;
    private bool NotAtEnd => !IsAtEnd;
}