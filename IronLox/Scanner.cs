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
                if(Match('/')) // line comment 
                {
                    while (Peek() != '\n' && NotAtEnd) Advance();
                }
                else // division 
                {
                    AddToken(SLASH);
                }
                break;


            // Whitespace
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace
                break;

            // New Line
            case '\n':
                line++;
                break;

            case '"': StringToken(); break;

            default:

                if (IsDigit(c))
                {
                    NumberToken();
                }
                else if(IsAlpha(c))
                {
                    IdentifierToken();
                }
                else
                {
                    //TODO: "Coalescing a run of invalid characters into a single error would give a nicer user experience" -Bob
                    Lox.Error(line, $"Unexpexted character: {c}");
                }
                break;
        }
    }

    private bool IsAlpha(char c)
    {
        return  (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                 c == '_';
    }

    private bool IsDigit(char c) => c >= '0' && c <= '9';

    private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

    private void IdentifierToken()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        string text = source.SubStr(start, current);
        
        TokenType type = FindKeyword(text) ?? IDENTIFIER;

        AddToken(type);
    }

    private void NumberToken()
    {
        while (IsDigit(Peek())) Advance();

        if(Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek())) Advance();
        }

        AddToken(NUMBER, double.Parse(source.SubStr(start, current)));
    }

    // Note: doesn't support excape chars
    private void StringToken()
    {
        while (Peek() != '"' && NotAtEnd)
        {
            if (Peek() == '\n') line++; // support multiline strings for some reason :) 
            Advance();
        }

        if (IsAtEnd)
        {
            Lox.Error(line, "Unterminated string.");
            return;
        }

        // Eat the closing ".
        Advance();

        // Trim surrounding quotes
        string val = source.SubStr(start + 1, current - 1);
        AddToken(STRING, val);
    }

    private TokenType? FindKeyword(string lexeme) =>
        lexeme switch
        {
            "and"   => AND,
            "class" => CLASS,
            "else"  => ELSE,
            "false" => FALSE,
            "for"   => FOR,
            "fun"   => FUN,
            "if"    => IF,
            "nil"   => NIL,
            "or"    => OR,
            "print" => PRINT,
            "return"=> RETURN,
            "super" => SUPER,
            "this"  => THIS,
            "true"  => TRUE,
            "var"   => VAR,
            "while" => WHILE,
            _ => null

        };

    //-----------SCANNER MECHANICS---------------//

    private char Advance() => source[current++];
    

    private void AddToken(TokenType type, object? literal = null)
    {
        string text = source.SubStr(start, current);
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

    private char PeekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }

    private bool IsAtEnd => current >= source.Length;
    private bool NotAtEnd => !IsAtEnd;
}