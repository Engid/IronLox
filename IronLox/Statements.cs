namespace IronLox;

public abstract record Stmt
{
}

public record StmtExpression(Expr expression) : Stmt;

public record Print(Expr expression) : Stmt;

public static class StmtExtensions
{

}
