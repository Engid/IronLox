using System;
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

        foreach(var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string msg)
    {
        ReportError(line, "", msg);
    }
    static void ReportError(int line, string where, string msg)
    {
        //todo:
        /*
            Error: Unexpected "," in argument list.

            15 | function(first, second,);
                                       ^-- Here.
         */

        System.Console.Error.WriteLine($"[line {line}] Error{}: {msg}");
        _hadError = true;
    }
}


