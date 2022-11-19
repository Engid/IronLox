using static IronLox.TokenType;

namespace IronLox;

public class Interpreter
{
    public static object? EvalUnary(Unary u)
    {
        var right = u.Right.Eval();

        switch(u.Operator.Type)
        {
            case MINUS:
                return -(double)right;
            
            case BANG:
                return !IsTruthy(u);
        }

        return null;
    }

    private static bool IsTruthy(object o) => o switch
        {
            null => false,
            bool b => b, 
            _ => true,
        };
    

    public static object? EvalBinary(Binary expr)
    {
        var left = expr.Left.Eval();
        var right = expr.Right.Eval();

        switch (expr.Operator.Type)
        {
            case MINUS:
                return (double)left - (double)right;

        }

        return null;
    }

    public static object? EvalGrouping(Grouping g) => g.Expression.Eval();

    public static object? EvalLiteral(Literal l) => l.Value;
}


public static class ExprInterpExt
{
    public static object? Eval(this Expr expr) =>
        expr switch
        {
            Binary b => Interpreter.EvalBinary(b),
            Grouping g => Interpreter.EvalGrouping(g),
            Literal l => Interpreter.EvalLiteral(l),
            Unary u => Interpreter.EvalUnary(u),
        }; 

}
