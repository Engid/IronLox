using static IronLox.TokenType;

namespace IronLox;

public class Interpreter
{
    public static object? EvalUnary(Unary u)
    {
        var right = u.Right.Eval();

        switch (u.Operator.Type)
        {
            case MINUS:
                var r = CheckNumberOperand(u.Operator, right);
                return -(double)r;


            case BANG:
                return !IsTruthy(u);
        }

        return null;
    }

    public static object? EvalBinary(Binary expr)
    {
        // TODO: make a better error handling system
        var left = expr.Left.Eval();
        var right = expr.Right.Eval();

        switch (expr.Operator.Type)
        {
            case MINUS:
                {
                    var (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return (double)l - (double)r;
                }
            case PLUS:
                if (left is double && right is double)
                    return (double)left + (double)right;

                if (left is string && right is string)
                    return (string)left + (string)right;

                throw new Exception("Operands must be two numbers or two strings");

            case SLASH:
                {

                }
            case STAR:
                {

                }

            case GREATER:
                {
                    var (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return (double)l > (double)r;
                }
            case LESS:
                {
                    var (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return (double)l < (double)r;
                }
            case GREATER_EQUAL:
                {
                    var (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return (double)l <= (double)r;
                }
            case LESS_EQUAL:
                {
                    var (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return (double)l >= (double)r;
                }

            case BANG_EQUAL: return !IsEqual(left, right);
            case EQUAL_EQUAL: return IsEqual(left, right);
        }

        return null;
    }

    public static object? EvalGrouping(Grouping g) => g.Expression.Eval();

    public static object? EvalLiteral(Literal l) => l.Value;


    private static bool IsTruthy(object? o) => o switch
    {
        null => false,
        bool b => b,
        _ => true,
    };

    private static bool IsEqual(object? a, object? b) => a switch
    {
        null => b == null,
        _ => a.Equals(b),
    };

    private static double CheckNumberOperand(Token op, object? operand)
    {
        if (operand is double d) return d;

        throw new RuntimeError(op, "Operand must be a number");
    }

    private static (double, double) CheckNumberOperands(Token op, object? left, object? right)
    {
        if (left is double a && right is double b) return (a, b);

        throw new RuntimeError(op, "Operands must be a number");
    }
}

public class RuntimeError : Exception
{
    readonly public Token Token;

    public RuntimeError(Token t, string msg) : base(msg)
    {
        Token = t;
    }
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
