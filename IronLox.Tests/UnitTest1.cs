using System.Linq.Expressions;
using Xunit;

using IronLox;
using static IronLox.TokenType;

namespace IronLox.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void PlusExprPrints()
        {
            var expr = new Binary(new Literal(1), new Token(TokenType.PLUS, 1, "+"), new Literal(2));

            var exprStr = expr.PrintAst();

            Assert.Equal("(+ 1 2)", exprStr);
        }

        [Fact]
        public void MultiplicationExpr()
        {
            var expr = new Binary(
                new Unary(new Token(MINUS, 1, "-"), new Literal(123)), 
                new Token(STAR, 1, "*"), 
                new Grouping(new Literal(45.67)));

            var exprStr = expr.PrintAst();

            Assert.Equal("(* (- 123) (group 45.67))", exprStr);
        }
    }
}