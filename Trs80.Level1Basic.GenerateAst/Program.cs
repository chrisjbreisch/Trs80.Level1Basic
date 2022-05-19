using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Trs80.Level1Basic.Common.Extensions;

namespace Trs80.Level1Basic.GenerateAst;

internal static class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: generateAst <output directory>");
            Environment.Exit(64);
        }

        string outputDir = args[0];
        DefineAst(outputDir, "Expression", new List<string>
        {
            "Array      : Token name, Expression index",
            "Assign     : Token name, Expression value",
            "Binary     : Expression left, Token binaryOperator, Expression right",
            "Call       : Token callee, List<Expression> arguments",
            "Grouping   : Expression expression",
            "Identifier : Token name",
            "Literal    : dynamic value",
            "Unary      : Token unaryOperator, Expression right",
        });

        DefineAst(outputDir, "Statement", new List<string>
        {
            "Cls",
            "Compound               : LinkedList<Statement> statements",
            "Cont",
            "Data                   : List<Expression> dataElements",
            "Delete                 : int lineToDelete",
            "End",
            "For                    : Expression identifier, Expression startValue, Expression endValue, Expression stepValue",
            "Gosub                  : Expression location",
            "Goto                   : Expression location",
            "If                     : Expression condition, Statement thenBranch",
            "Input                  : List<Expression> expressions, bool writeNewline",
            "Let                    : Expression variable, Expression initializer",
            "List                   : Expression startAtLineNumber",
            "Load                   : Expression path",
            "Merge                  : Expression path",
            "New",
            "Next                   : Expression variable",
            "On                     : Expression selector, List<Expression> locations, bool isGosub",
            "Print                  : Expression atPosition, List<Expression> expressions, bool writeNewline",
            "Replace                : ParsedLine line",
            "Read                   : List<Expression> variables",
            "Rem                    : Literal remark",
            "Restore",
            "Return",
            "Run                    : Expression startAtLineNumber",
            "Save                   : Expression path",
            "StatementExpression    : Expression expression",
            "Stop",
        });
    }

    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        DefineVisitorInterface(outputDir, baseName, types);
        DefineBaseClass(outputDir, baseName);
        DefineSubClasses(outputDir, baseName, types);
    }

    private static void DefineSubClasses(string outputDir, string baseName, List<string> types)
    {
        foreach (string type in types)
        {
            string[] typeInfo = type.Split(":");

            string className = typeInfo[0].Trim();
            string fields = typeInfo.Length > 1 ? type.Split(":")[1].Trim() : string.Empty;
            DefineClass(outputDir, baseName, className, fields);
        }
    }

    private static void DefineBaseClass(string outputDir, string baseName)
    {
        string path = $"{outputDir}\\{baseName}s\\{baseName}.cs";
        using var writer = new StreamWriter(path);


        WriteHeaders(baseName, writer);
        writer.WriteLine($"public abstract class {baseName}");
        writer.WriteLine("{");
        if (baseName.Contains("Statement"))
        {
            writer.WriteLine("    public int LineNumber { get; set; } = -1;");
            writer.WriteLine("    public string SourceLine { get; set; }");
            writer.WriteLine("    public Statement Next { get; set; }");
            writer.WriteLine("    public Statement Parent { get; set; }");
            writer.WriteLine();
        }

        writer.WriteLine("    public abstract T Accept<T>(IVisitor<T> visitor);");
        writer.WriteLine("}");
        WriteEnd(writer);
    }

    private static void WriteEnd(TextWriter writer)
    {
        writer.Flush();
        writer.Close();
    }

    private static void WriteHeaders(string baseName, TextWriter writer)
    {
        WriteDisclaimer(writer);

        writer.WriteLine();
        writer.WriteLine("using System.Collections.Generic;");

        writer.WriteLine(baseName.Contains("Statement")
            ? "using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;"
            : "using Trs80.Level1Basic.VirtualMachine.Scanner;");

        writer.WriteLine();
        writer.WriteLine($"namespace Trs80.Level1Basic.VirtualMachine.Parser.{baseName}s;");
        writer.WriteLine();
    }

    private static void WriteDisclaimer(TextWriter writer)
    {
        writer.WriteLine("//");
        writer.WriteLine("//");
        writer.WriteLine("// This file was automatically generated by generateAst");
        writer.WriteLine($"// at {DateTime.UtcNow} UTC. Do not modify.");
        writer.WriteLine("//");
        writer.WriteLine("//");
    }

    private static void DefineVisitorInterface(string outputDir, string baseName, List<string> types)
    {
        string path = $"{outputDir}\\{baseName}s\\IVisitor.cs";
        using var writer = new StreamWriter(path);

        WriteDisclaimer(writer);

        writer.WriteLine($"namespace Trs80.Level1Basic.VirtualMachine.Parser.{baseName}s;");
        writer.WriteLine();
        writer.WriteLine("public interface IVisitor<T>");
        writer.WriteLine("{");

        foreach (string type in types)
        {
            string typeName = type.Split(":")[0].Trim();
            writer.WriteLine($"    T Visit{typeName}{baseName}({typeName} {baseName.SeparateWordsByCase('_').ToCamelCase()});");
        }
        writer.WriteLine("}");
        WriteEnd(writer);
    }

    private static void DefineClass(string outputDir, string baseName, string className, string fieldList)
    {
        string path = $"{outputDir}\\{baseName}s\\{className}.cs";
        using var writer = new StreamWriter(path);

        WriteHeaders(baseName, writer);
        string[] fields = fieldList.Split(", ");
        writer.WriteLine($"public class {className} : {baseName}");
        writer.WriteLine("{");

        foreach (string field in fields.Where(s => !string.IsNullOrEmpty(s)))
        {
            string[] fieldPieces = field.Split(" ");
            writer.WriteLine($"    public {fieldPieces[0]} {fieldPieces[1].SeparateWordsByCase('_').ToPascalCase()} {{ get; init; }}");
        }

        writer.WriteLine();
        writer.WriteLine($"    public {className}({fieldList})");
        writer.WriteLine("    {");

        foreach (string field in fields.Where(s => !string.IsNullOrEmpty(s)))
        {
            string name = field.Split(" ")[1];
            writer.WriteLine($"        {name.SeparateWordsByCase('_').ToPascalCase()} = {name};");
        }

        writer.WriteLine("    }");

        writer.WriteLine();
        writer.WriteLine("    public override T Accept<T>(IVisitor<T> visitor)");
        writer.WriteLine("    {");
        writer.WriteLine($"        return visitor.Visit{className}{baseName}(this);");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        WriteEnd(writer);
    }
}