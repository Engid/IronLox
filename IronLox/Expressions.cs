using System.Text;

namespace IronLox; //test

public abstract record Expr { }

public record Literal(object? Value) : Expr;

public record Variable(Token Name) : Expr;

public record Unary(Token Operator, Expr Right) : Expr;

public record Binary(Expr Left, Token Operator, Expr Right) : Expr;

public record Grouping(Expr Expression) : Expr;


/// <summary>
///  This extension class mimics the visitor pattern
///  but is more concise and easier to use.
/// </summary>
public static class ExprProcessor
{
    public static string PrintAst(this Expr expr) =>
        expr switch
        {
            Binary b => Parenthesize(b.Operator.Lexeme, b.Left, b.Right),
            Grouping g => Parenthesize("group", g.Expression),
            Literal l => l.Value is object o ? o.ToString() : "nil",
            Unary u => Parenthesize(u.Operator.Lexeme, u.Right),
            null => "",
            _ => ""
        } ?? ""; // why do i need this to fix warnings???

    private static string Parenthesize(string name, params Expr[] expr)
    {
        var sb = new StringBuilder();

        sb.Append('(').Append(name);

        foreach (var e in expr)
        {
            sb.Append(' ');
            sb.Append(e.PrintAst());
        }
        sb.Append(')');

        return sb.ToString();
    }
}

