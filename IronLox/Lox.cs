namespace IronLox;

public class Lox
{
    static bool _hadError = false;
    static bool _hadRuntimeError = false;

    private static readonly Interpreter _interpreter = new Interpreter();

    public static void StartUp(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: IronLox [script]");
            System.Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine($"{error.Message} \n[line {error.Token.Line}]");
        _hadRuntimeError = true;
    }

    private static void RunFile(string path)
    {
        var text = File.ReadAllText(path);
        Run(text);

        if (_hadError) System.Environment.Exit(65);
        if (_hadRuntimeError) System.Environment.Exit(70);
    }

    private static void RunPrompt()
    {
        for (; ; )
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            if (input == null) break;
            Run(input);
            _hadError = false;
        }
    }

    private static void Run(string source)
    {
        Scanner scanner = new(source);
        List<Token> tokens = scanner.ScanTokens();
        Parser parser = new(tokens);
        List<Stmt> statements = parser.Parse();

        if (_hadError) return;

        _interpreter.Interpret(statements);

        //Console.WriteLine(expression.PrintAst());
    }

    public static void Error(int line, string msg)
    {
        ReportError(line, "", msg);
    }

    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            ReportError(token.Line, " at end", message);
        }
        else
        {
            ReportError(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    static void ReportError(int line, string where, string msg)
    {
        //todo:
        /*
            Error: Unexpected "," in argument list.

            15 | function(first, second,);
                                       ^-- Here.
         */

        Console.Error.WriteLine($"[line {line}] Error{where}: {msg}");
        _hadError = true;
    }
}


