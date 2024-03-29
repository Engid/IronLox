﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IronLox;

public class Lox
{
    static bool _hadError = false;

    public static void StartUp(string[] args)
    {
        if(args.Length > 1)
        {
            System.Console.WriteLine("Usage: IronLox [script]");
            Environment.Exit(64);
        } else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string path) 
    { 
        var text = File.ReadAllText(path);
        Run(text);

        if(_hadError) Environment.Exit(65);
    }

    private static void RunPrompt()
    {
        for (;;)
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
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        Parser parser = new Parser(tokens);
        Expr? expression = parser.Parse();

        if (_hadError || expression is null) return;

        Console.WriteLine(expression.PrintAst());
    }

    public static void Error(int line, string msg)
    {
        ReportError(line, "", msg);
    }

    public static void Error(Token token, string message)
    {
        if(token.Type == TokenType.EOF)
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

        System.Console.Error.WriteLine($"[line {line}] Error{where}: {msg}");
        _hadError = true;
    }
}


