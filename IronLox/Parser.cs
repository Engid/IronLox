using System.Collections.Concurrent;
using System.Data;
using static IronLox.TokenType;


namespace IronLox;
public sealed class Parser
{
    private class ParseError(string message) : Exception(message);

    private readonly List<Token> tokens;
    private int current = 0;

    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        // try
        // {
        //     return Expression();
        // }
        // catch (ParseError error)
        // {
        //     return null;
        // }

        List<Stmt> statements = new();

        while (!IsAtEnd)
        {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Stmt Declaration()
    {
        try
        {
            if (Match(VAR)) return VarDecl();
            return Statement();
        }
        catch (ParseError error)
        {
            Synchronize();
            return null; // hey why we gotta do this Bob?
        }
    }

    private Stmt VarDecl()
    {
        Token name = Consume(IDENTIFIER, "Expect variable name.");

        Expr? initializer = null;
        if (Match(EQUAL))
        {
            initializer = Expression();
        }

        Consume(SEMICOLON, "Expect ';' after variable declaration");
        return new Var(name, initializer);
    }

    private Stmt Statement()
    {
        if (Match(PRINT)) return PrintStatement();

        return ExpressionStatement();
    }

    private Stmt PrintStatement()
    {
        Expr value = Expression();
        Consume(SEMICOLON, "Expect ';' after value.");
        return new Print(value);
    }

    private Stmt ExpressionStatement()
    {
        Expr expr = Expression();
        Consume(SEMICOLON, "Expect ';' after value.");
        return new StmtExpression(expr);
    }

    private Expr Expression() => Equality();

    private Expr Equality()
    {
        Expr expr = Comparison();

        while (Match(BANG_EQUAL, EQUAL_EQUAL))
        {
            Token op = Previous;
            Expr right = Comparison();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Comparison()
    {
        Expr expr = Term();

        while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
        {
            Token op = Previous;
            Expr right = Term();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Term()
    {
        Expr expr = Factor();

        while (Match(MINUS, PLUS))
        {
            Token op = Previous;
            Expr right = Factor();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }
    private Expr Factor()
    {
        Expr expr = Unary();

        while (Match(SLASH, STAR))
        {
            Token op = Previous;
            Expr right = Unary();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }
    private Expr Unary()
    {
        if (Match(BANG, MINUS))
        {
            Token op = Previous;
            Expr right = Unary();
            return new Unary(op, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(FALSE)) return new Literal(false);
        if (Match(TRUE)) return new Literal(true);
        if (Match(NIL)) return new Literal(null);

        if (Match(NUMBER, STRING))
        {
            return new Literal(Previous.Literal);
        }

        if (Match(IDENTIFIER))
        {
            return new Variable(Previous);
        }

        if (Match(LEFT_PAREN))
        {
            Expr expr = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after expression.");
            return new Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }


    private bool Match(params TokenType[] tokens)
    {
        foreach (TokenType t in tokens)
        {
            if (Check(t))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd) return false;

        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (NotAtEnd) current++;
        return Previous;
    }

    private bool IsAtEnd => Peek().Type == EOF;
    private bool NotAtEnd => !IsAtEnd;
    private Token Peek() => tokens[current];
    private Token Previous => tokens[current - 1];

    private ParseError Error(Token token, string message)
    {
        Lox.Error(token, message);
        return new ParseError(message);
    }

    private void Synchronize()
    {
        Advance();

        while (NotAtEnd)
        {
            if (Previous.Type == SEMICOLON) return;

            switch (Peek().Type)
            {
                case CLASS:
                case FUN:
                case VAR:
                case FOR:
                case IF:
                case WHILE:
                case PRINT:
                case RETURN:
                    return;
            }

            Advance();
        }
    }
}
