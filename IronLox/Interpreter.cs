using static IronLox.TokenType;

namespace IronLox;

public class Interpreter
{
    private Environment _env = new();

    public void Interpret(IEnumerable<Stmt> statements)
    {
        try
        {
            foreach (var stmt in statements)
            {
                Execute(stmt);
            }
        }
        catch (RuntimeError e)
        {
            Lox.RuntimeError(e);
        }
    }
    public object? EvalUnary(Unary u)
    {
        var right = Eval(u.Right);

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

    public object? EvalBinary(Binary expr)
    {
        var left = Eval(expr.Left);
        var right = Eval(expr.Right);

        switch (expr.Operator.Type)
        {
            case MINUS:
                {
                    var (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return l - r;
                }
            case PLUS:
                {
                    if (left is double && right is double)
                    {
                        return (double)left + (double)right;
                    }

                    if (left is string && right is string)
                    {
                        return (string)left + (string)right;
                    }

                    throw new RuntimeError(expr.Operator, "Plus requires numbers or strings");
                }

            case SLASH:
                {
                    var (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return (double)l / (double)r;
                }
            case STAR:
                {
                    var (l, r) = CheckNumberOperands(expr.Operator, left, right);
                    return (double)l * (double)r;
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

    public object? EvalGrouping(Grouping g) => Eval(g.Expression);

    public object? EvalLiteral(Literal l) => l.Value;


    // ---- Statement Evaluation ----

    public void EvalVarStmt(Var stmt)
    {
        object? value = null;
        if (stmt.Init != null)
        {
            value = Eval(stmt.Init);
        }
        _env.Define(stmt.Name.Lexeme, value);

        return;
    }

    public void EvalPrint(Print p)
    {
        object? value = Eval(p.expression);
        Console.WriteLine(Stringify(value));
    }

    public static string Stringify(object? o)
    {
        // TODO: make better formatting for decimal
        return o?.ToString() ?? "nil";
    }

    // NOTE: we use pattern matching instead of visitor pattern. 
    public object? Eval(Expr expr) => expr switch
    {
        Binary b => EvalBinary(b),
        Grouping g => EvalGrouping(g),
        Literal l => EvalLiteral(l),
        Unary u => EvalUnary(u),
        Variable v => _env.Get(v.Name),
        _ => throw new EvalError($"Expression not defined: {expr}")
    };

    // NOTE: we use pattern matching instead of visitor pattern.
    public void Execute(Stmt stmt)
    {
        switch (stmt)
        {
            case Print p:
                EvalPrint(p);
                break;

            case StmtExpression se:
                Eval(se.expression);
                break;

            case Var v:
                EvalVarStmt(v);
                break;

            default: throw new Exception($"Statement not valid {stmt}");
        }
    }

    // -----PRIVATES-----

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

public class EvalError(string msg) : Exception(msg);
