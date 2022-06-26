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
            "Array      : Token name, Expression index, int linePosition",
            "Binary     : Expression left, Token binaryOperator, Expression right, int linePosition",
            "Call       : Callable callee, List<Expression> arguments, int linePosition",
            "Grouping   : Expression expression, int linePosition",
            "Identifier : Token name, int linePosition",
            "Literal    : dynamic value, string upperValue, int linePosition",
            "Selector   : Expression expression, int linePosition",
            "Unary      : Token unaryOperator, Expression right, int linePosition",
        });

        DefineAst(outputDir, "Statement", new List<string>
        {
            "Cls",
            "Compound               : CompoundStatementList statements",
            "Cont",
            "Data                   : List<Expression> dataElements",
            "Delete                 : int lineToDelete",
            "End",
            "For                    : Token identifierName, Expression identifier, Expression startValue, Expression endValue, Expression stepValue",
            "Gosub                  : Expression location, int linePosition",
            "Goto                   : Expression location, int linePosition",
            "If                     : Expression condition, int thenPosition, CompoundStatementList thenBranch, ParseException thenException",
            "Input                  : List<Expression> expressions",
            "Let                    : Expression variable, Expression initializer",
            "List                   : Expression startAtLineNumber",
            "Load                   : Expression path",
            "Merge                  : Expression path",
            "New",
            "Next                   : Token identifierName, Expression identifier",
            "On                     : Selector selector, List<Expression> locations, List<int> linePositions, bool isGosub",
            "Print                  : Expression atPosition, List<Expression> expressions, bool writeNewline",
            "Replace                : IStatement statement",
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
        if (baseName.Contains("Statement"))
            DefineInterface(outputDir, baseName);
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

    private static void DefineInterface(string outputDir, string baseName)
    {
        string path = $"{outputDir}\\{baseName}s\\I{baseName}.cs";
        using var writer = new StreamWriter(path);

        WriteHeaders(baseName, writer);
        writer.WriteLine($"public interface I{baseName}");
        writer.WriteLine("{");
        writer.WriteLine("    int LineNumber { get; set; }");
        writer.WriteLine("    string SourceLine { get; set; }");
        writer.WriteLine("    IStatement Next { get; set; }");
        writer.WriteLine("    IStatement Previous { get; set; }");
        writer.WriteLine("    ParseException ParseException { get; set; }");
        writer.WriteLine();

        writer.WriteLine("    T Accept<T>(IVisitor<T> visitor);");
        writer.WriteLine("}");
        WriteEnd(writer);
    }

    private static void DefineBaseClass(string outputDir, string baseName)
    {
        string path = $"{outputDir}\\{baseName}s\\{baseName}.cs";
        using var writer = new StreamWriter(path);
        string implements = string.Empty;

        if (baseName.Contains("Statement"))
            implements = $": I{baseName}";

        WriteHeaders(baseName, writer);
        writer.WriteLine($"public abstract class {baseName}{implements}");
        writer.WriteLine("{");
        writer.WriteLine("    public ParseException ParseException { get; set; }");
        if (baseName.Contains("Statement"))
        {
            writer.WriteLine("    public int LineNumber { get; set; } = -1;");
            writer.WriteLine("    public string SourceLine { get; set; }");
            writer.WriteLine("    public IStatement Next { get; set; }");
            writer.WriteLine("    public IStatement Previous { get; set; }");
            writer.WriteLine("    public IStatement Enclosing { get; set; }");
            writer.WriteLine();
        }
        else
        {
            writer.WriteLine("    public int LinePosition { get; set; } = 0;");
            writer.WriteLine();
            writer.WriteLine("    protected Expression(int linePosition)");
            writer.WriteLine("    {");
            writer.WriteLine("        LinePosition = linePosition;");
            writer.WriteLine("    }");
            writer.WriteLine();
        }

        writer.WriteLine("    protected void CheckExceptions()");
        writer.WriteLine("    {");
        writer.WriteLine("        if (ParseException != null) throw ParseException;");
        writer.WriteLine("    }");
        writer.WriteLine();
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
        writer.WriteLine();

        writer.WriteLine("using Trs80.Level1Basic.VirtualMachine.Exceptions;");
        writer.WriteLine("using Trs80.Level1Basic.VirtualMachine.Scanner;");

        writer.WriteLine(baseName.Contains("Statement")
            ? "using Trs80.Level1Basic.VirtualMachine.Parser.Expressions;"
            : "using Trs80.Level1Basic.VirtualMachine.Machine;");

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
        writer.WriteLine("public interface IVisitor<out T>");
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
            if (baseName == "Expression" && field == "int linePosition") continue;
            string[] fieldPieces = field.Split(" ");
            writer.WriteLine($"    public {fieldPieces[0]} {fieldPieces[1].SeparateWordsByCase('_').ToPascalCase()} {{ get; init; }}");
        }

        writer.WriteLine();
        
        writer.WriteLine(baseName == "Expression"
            ? $"    public {className}({fieldList}) : base(linePosition)"
            : $"    public {className}({fieldList})");

        writer.WriteLine("    {");

        foreach (string field in fields.Where(s => !string.IsNullOrEmpty(s)))
        {
            if (baseName == "Expression" && field == "int linePosition") continue;
            string name = field.Split(" ")[1];
            writer.WriteLine($"        {name.SeparateWordsByCase('_').ToPascalCase()} = {name};");
        }

        writer.WriteLine("    }");

        writer.WriteLine();
        writer.WriteLine("    public override T Accept<T>(IVisitor<T> visitor)");
        writer.WriteLine("    {");
        writer.WriteLine("        CheckExceptions();");
        writer.WriteLine($"        return visitor.Visit{className}{baseName}(this);");
        writer.WriteLine("    }");
        writer.WriteLine("}");
        WriteEnd(writer);
    }
}