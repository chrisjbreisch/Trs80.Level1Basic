using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Trs80.Level1Basic.Extensions;

//using Trs80.Level1Basic.Services.Extensions;

namespace GenerateAst;
internal class Program
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
            "BasicArray : Token name, Expression index",
            "Assign     : Token name, Expression value",
            "Binary     : Expression left, Token operatorType, Expression right",
            "Call       : Token name, List<Expression> arguments",
            "Grouping   : Expression expression",
            "Literal    : dynamic value",
            "Unary      : Token operatorType, Expression right",
            "Identifier : Token name",
        });

        DefineAst(outputDir, "Statement", new List<string>
        {
            "Cls",
            "Cont",
            "Data                   : List<Expression> dataElements",
            "Delete                 : int lineToDelete",
            "End",
            "For                    : Expression variable, Expression startValue, Expression endValue, Expression stepValue",
            "Goto                   : Expression location",
            "Gosub                  : Expression location",
            "If                     : Expression condition, List<Statement> thenStatements",
            "Input                  : List<Expression> expressions, bool writeNewline",
            "Let                    : Expression variable, Expression initializer",
            "List                   : Expression startAtLineNumber",
            "Load                   : Expression path",
            "New",
            "Next                   : Expression variable",
            "On                     : Expression selector, List<Expression> locations, bool isGosub",
            "Print                  : Expression atPosition, List<Expression> expressions, bool writeNewline",
            "Replace                : Line line",
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
        writer.WriteLine($"    public abstract class {baseName}");
        writer.WriteLine("    {");
        string returnType = "dynamic";
        if (baseName.Contains("Statement"))
        {
            writer.WriteLine("        public int LineNumber { get; set; }");
            writer.WriteLine("        public string SourceLine { get; set; }");
            writer.WriteLine("        public Guid UniqueIdentifier { get; set; }");
            writer.WriteLine("        public Statement Next { get; set; }");
            writer.WriteLine();
            returnType = "void";
        }

        writer.WriteLine($"        public abstract {returnType} Accept(I{baseName}Visitor visitor);");
        writer.WriteLine("    }");
        WriteEnd(writer);
    }

    private static void WriteEnd(StreamWriter writer)
    {
        writer.WriteLine("}");
        writer.Flush();
        writer.Close();
    }

    private static void WriteHeaders(string baseName, StreamWriter writer)
    {
        WriteDisclaimer(writer);

        writer.WriteLine();
        writer.WriteLine("using System;");
        writer.WriteLine("using System.Collections.Generic;");

        if (baseName.Contains("Statement"))
            writer.WriteLine("using Trs80.Level1Basic.Services.Parser.Expressions;");

        writer.WriteLine("using Trs80.Level1Basic.Domain;");
        writer.WriteLine();
        writer.WriteLine($"namespace Trs80.Level1Basic.Services.Parser.{baseName}s");
        writer.WriteLine("{");
    }

    private static void WriteDisclaimer(StreamWriter writer)
    {
        writer.WriteLine("//");
        writer.WriteLine("// This file is automatically generated. Do not modify.");
        writer.WriteLine("//");
    }

    private static void DefineVisitorInterface(string outputDir, string baseName, List<string> types)
    {
        string path = $"{outputDir}\\{baseName}s\\I{baseName}Visitor.cs";
        using var writer = new StreamWriter(path);

        WriteDisclaimer(writer);

        writer.WriteLine($"namespace Trs80.Level1Basic.Services.Parser.{baseName}s");
        writer.WriteLine("{");
        writer.WriteLine($"    public interface I{baseName}Visitor");
        writer.WriteLine("    {");

        foreach (string type in types)
        {
            string returnType = baseName.Contains("Statement") ? "void" : "dynamic";

            string typeName = type.Split(":")[0].Trim();
            writer.WriteLine($"        {returnType} Visit{typeName}{baseName}({typeName} root);");
        }
        writer.WriteLine("    }");
        WriteEnd(writer);
    }

    private static void DefineClass(string outputDir, string baseName, string className, string fieldList)
    {
        string path = $"{outputDir}\\{baseName}s\\{className}.cs";
        using var writer = new StreamWriter(path);

        WriteHeaders(baseName, writer);
        string[] fields = fieldList.Split(", ");
        writer.WriteLine($"    public class {className} : {baseName}");
        writer.WriteLine("    {");

        foreach (string field in fields.Where(s => !string.IsNullOrEmpty(s)))
        {
            string[] fieldPieces = field.Split(" ");
            writer.WriteLine($"        public {fieldPieces[0]} {fieldPieces[1].ToPascalCase()} {{ get; }}");
        }

        writer.WriteLine();
        writer.WriteLine($"        public {className}({fieldList})");
        writer.WriteLine("        {");

        foreach (string field in fields.Where(s => !string.IsNullOrEmpty(s)))
        {
            string name = field.Split(" ")[1];
            writer.WriteLine($"            {name.ToPascalCase()} = {name};");
        }

        writer.WriteLine("        }");

        string returnType = baseName.Contains("Statement") ? "void" : "dynamic";
        string returnStatement = baseName.Contains("Statement") ? "" : "return ";
        writer.WriteLine();
        writer.WriteLine($"        public override {returnType} Accept(I{baseName}Visitor visitor)");
        writer.WriteLine("        {");
        writer.WriteLine($"            {returnStatement}visitor.Visit{className}{baseName}(this);");
        writer.WriteLine("        }");
        writer.WriteLine("    }");
        WriteEnd(writer);
    }
}