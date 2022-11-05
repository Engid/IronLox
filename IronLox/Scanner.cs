namespace IronLox;

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

        tokens.Add(new(TokenType.EOF, line, "", null));
        return tokens;
    }

    private void ScanToken()
    {
        char c = Advance();

        switch (c)
        {
            case '(': AddToken(TokenType.LEFT_PAREN);   break;
            case ')': AddToken(TokenType.RIGHT_PAREN);  break;
            case '{': AddToken(TokenType.LEFT_BRACE);   break;
            case '}': AddToken(TokenType.RIGHT_BRACE);  break;
            case ',': AddToken(TokenType.COMMA);        break;
            case '.': AddToken(TokenType.DOT);          break;
            case '-': AddToken(TokenType.MINUS);        break;
            case '+': AddToken(TokenType.PLUS);         break;
            case ';': AddToken(TokenType.SEMICOLON);    break;
            case '*': AddToken(TokenType.STAR);         break;

            default: break;
        }


    }

    private char Advance() => source[current++];
    

    private void AddToken(TokenType type, object? literal = null)
    {
        string text = source.Substring(start, current);
        tokens.Add(new(type, line, text, literal));
    }


    private bool IsAtEnd => current >= source.Length;
    private bool NotAtEnd => !IsAtEnd;
}