using System.Collections.Generic;
using System.IO;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Application;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.Console;
using Trs80.Level1Basic.Interpreter.Interpreter;
using Trs80.Level1Basic.Interpreter.Parser;
using Trs80.Level1Basic.Interpreter.Scanner;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class InterpreterTest
{
    private IScanner? _scanner;
    private IParser? _parser;
    private IBasicInterpreter? _interpreter;
    private IBasicEnvironment? _environment;
    private IConsole? _console;
    private readonly StringWriter _sw = new();

    [TestInitialize]
    public void Initialize()
    {
        var bootstrapper = new Bootstrapper();
        IAppSettings? appSettings = bootstrapper.AppSettings;
        ILoggerFactory? loggerFactory = bootstrapper.LogFactory;

        _console = new Console.Console(appSettings, loggerFactory, false);
        _console.Out = _sw;

        IBuiltinFunctions builtins = new BuiltinFunctions();
        _scanner = new Scanner.Scanner(builtins);
        _parser = new Parser.Parser(builtins);
        IProgram program = new Program();
        _environment = new BasicEnvironment(_console, _parser, _scanner, builtins, program);
        _interpreter = new BasicInterpreter(_console, _environment);
    }

    private void ExecuteLine(string input)
    {
        List<Token> tokens = _scanner!.ScanTokens(input);
        ParsedLine parsedLine = _parser!.Parse(tokens);
        _interpreter!.Interpret(parsedLine);
    }

    private void RunProgram(List<string> program)
    {
        ExecuteLine("new");

        ExecuteStatements(program);

        ExecuteLine("run");
    }

    private void ExecuteStatements(List<string> statements)
    {
        foreach (string line in statements)
            ExecuteLine(line);
    }

    [TestMethod]
    public void Interpreter_Can_Run_HelloWorld()
    {
        var program = new List<string> {
            "10 print \"Hello, World!\""
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Hello, World!");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Simple_Assignment()
    {
        var program = new List<string> {
            "10 i=3",
            "20 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Multiple_Assignments()
    {
        var program = new List<string> {
            "10 i=3",
            "15 i=7",
            "20 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 7 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Do_Simple_Math()
    {
        var program = new List<string> {
            "10 i=3",
            "20 j=4",
            "30 n=i*j",
            "40 print n"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 12 ");
        sr.ReadToEnd();
    }


    [TestMethod]
    public void Interpreter_Can_Do_Harder_Math()
    {
        var program = new List<string> {
            "10 c=25",
            "20 f=(9/5) * c + 32",
            "30 print f"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 77 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Goto()
    {
        var program = new List<string> {
            "10 i=3",
            "20 goto 40",
            "30 i=7",
            "40 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_End()
    {
        var program = new List<string> {
            "10 end",
            "20 print \"How did I get here?\""
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("");
        output = sr.ReadLine();
        output.Should().Be("READY");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Gosub()
    {
        var program = new List<string> {
            "10 i=3",
            "20 gosub 100",
            "30 print i",
            "40 end",
            "100 i = 7",
            "110 return"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 7 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Multi_Statement_Lines()
    {
        var program = new List<string> {
            "10 i=3 : print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Addition()
    {
        var program = new List<string> {
            "10 i=3",
            "20 i= i + 1",
            "30 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 4 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Handle_Lines_Inserted_In_The_Middle()
    {
        var program = new List<string> {
            "10 i=3",
            "20 print i",
            "15 i=i * 2"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 6 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Correct_Statement_After_Gosub()
    {
        var program = new List<string> {
            "10 i=3 : gosub 100 : i = i + 1",
            "20 print i",
            "30 end",
            "100 i = 5",
            "110 return"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 6 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Process_String_Variables()
    {
        var program = new List<string> {
            "10 A$=\"Chris\"",
            "20 print \"Hello, \";A$"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Hello, Chris");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Read_From_Mocked_Console()
    {
        using var input = new StringReader("Chris");
        _console!.In = input;

        var program = new List<string> {
            "10 input \"Enter your name\";A$",
            "20 print \"Hello, \";A$"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Enter your name?Hello, Chris");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement()
    {
        var program = new List<string> {
            "10 i = 5",
            "20 if i = 5 then 40",
            "30 i = 7",
            "40 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 5 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Goto()
    {
        var program = new List<string> {
            "10 i = 5",
            "20 if i = 5 goto 40",
            "30 i = 7",
            "40 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 5 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Print()
    {
        var program = new List<string> {
            "10 i = 5",
            "20 if i = 5 then print i;",
            "30 i = 7",
            "40 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 5  7 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Variable_Assignment()
    {
        var program = new List<string> {
            "10 i = 0 : s = 2",
            "20 if i < 0 then s = -1",
            "30 if i = 0 then s = 0",
            "40 if i > 0 then s = 1",
            "40 print s"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 0 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Multiple_Variable_Assignment()
    {
        var program = new List<string> {
            "10 i = 4: p=3.14159: e=2.71828",
            "20 if i = 4 then t=p:p=e:e=t",
            "30 print p;e"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 2.71828  3.14159 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_If_Statement_With_Invalid_Goto()
    {
        var program = new List<string> {
            "10 i = 4",
            "20 if i = 4 then 100: goto 200",
            "30 print \"IF is broken\"",
            "40 end",
            "100 print i",
            "110 end",
            "200 print \"THEN is broken\"",
            "210 end"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 4 ");

        output = sr.ReadLine();
        output.Should().Be("");

        output = sr.ReadLine();
        output.Should().Be("READY");

        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Commands1()
    {
        var statements = new List<string> {
            "print 3 * 4",
        };
        ExecuteStatements(statements);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 12 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Commands2()
    {
        var statements = new List<string> {
            "print 345/123",
        };
        ExecuteStatements(statements);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 2.80488 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Commands3()
    {
        var statements = new List<string> {
            "print (2/3)*(3/2)",
        };
        ExecuteStatements(statements);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 1 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Clears_Memory_Properly()
    {
        var statements = new List<string> {
            "new",
            "print a;b;c;d;e;f;g;h;i;j;k;l;m;n;o;p;q;r;s;t;u;v;w;x;y;z"
        };
        ExecuteStatements(statements);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Returns_Full_Memory_After_New()
    {
        var statements = new List<string> {
            "new",
            "print mem"
        };
        ExecuteStatements(statements);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 15871 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Counts_Static_Memory_Usage1()
    {
        var statements = new List<string> {
            "new",
            "10 a = 25",
            "print mem"
        };
        ExecuteStatements(statements);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 15861 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Counts_Static_Memory_Usage2()
    {
        var statements = new List<string> {
            "new",
            "10 a = 25",
            "20 print \"this example is to measure memory usage.\"",
            "print mem"
        };
        ExecuteStatements(statements);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 15809 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Handles_Line_Number_Zero()
    {
        var program = new List<string> {
            "0 let a = 3.14159",
            "1 print a",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3.14159 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_For_Loop()
    {
        var program = new List<string> {
            "10 for n = 1 to 10 : next n",
            "20 print n",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 11 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_For_Loop_With_Step()
    {
        var program = new List<string> {
            "10 for n = 1 to 10 step 2",
            "20 i = i + n",
            "30 next n",
            "40 print i"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 25 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_For_Loop_With_Negative_Step()
    {
        var program = new List<string> {
            "10 for n = 10 to 3 step -1",
            "30 next n",
            "40 print n"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 2 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_Nested_For_Loop()
    {
        var program = new List<string> {
            "10 a = 1",
            "30 for i = 1 to 2",
            "40 for j = 1 to 5",
            "50 a = a * 2",
            "60 next j",
            "70 next i",
            "80 print a"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 1024 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_On_Goto()
    {
        var program = new List<string> {
            "10 a = 4",
            "20 on a goto 100,200,300,400,500,600",
            "30 print a",
            "40 end",
            "100 a = a + 1: print a: end",
            "200 a = a - 1: print a: end",
            "300 a = a * 2: print a: end",
            "400 a = a / 2: print a: end",
            "500 a = a + 2: print a: end",
            "600 a = a * a: print a: end"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 2 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_On_Goto_With_Float()
    {
        var program = new List<string> {
            "10 a = 5.2",
            "20 on a goto 100,200,300,400,500,600",
            "30 print a",
            "40 end",
            "100 a = a + 1: print a: end",
            "200 a = a - 1: print a: end",
            "300 a = a * 2: print a: end",
            "400 a = a / 2: print a: end",
            "500 a = a + 2: print a: end",
            "600 a = a * a: print a: end"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 7.2 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_On_Gosub()
    {
        var program = new List<string> {
            "10 a = 4",
            "20 on a gosub 100,200,300,400,500,600",
            "30 print a",
            "40 end",
            "100 a = a + 1: return",
            "200 a = a - 1: return",
            "300 a = a * 2: return",
            "400 a = a / 2: return",
            "500 a = a + 2: return",
            "600 a = a * a: return"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 2 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_Data_And_Read()
    {
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 read a,b,c,d,e",
            "30 print e;d;c;b;a",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 5  4  3  2  1 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_Data_In_The_Middle()
    {
        var program = new List<string> {
            "20 read a,b,c,d,e",
            "25 data 1,2,3,4,5",
            "30 print e;d;c;b;a",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 5  4  3  2  1 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_Data_At_The_End()
    {
        var program = new List<string> {
            "20 read a,b,c,d,e",
            "30 print e;d;c;b;a",
            "40 data 1,2,3,4,5",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 5  4  3  2  1 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_Restore()
    {
        var program = new List<string> {
            "5 i = 0",
            "10 data 1, 2, 3, 4, 5",
            "20 for n = 1 to 5",
            "30 read a",
            "40 print a;",
            "50 next n",
            "60 i = i + 1",
            "70 restore",
            "80 if i = 1 then 10",
            "90 end"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 1  2  3  4  5  1  2  3  4  5 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Stores_Variables_Until_Explicitly_Cleared()
    {
        var program = new List<string> {
            "10 p = 3.14159"
        };
        RunProgram(program);

        ExecuteLine("print p");

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("");

        output = sr.ReadLine();
        output.Should().Be("READY");

        output = sr.ReadLine();
        output.Should().Be(" 3.14159 ");

        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Handles_Indirect_References_On_Input1()
    {
        using var input = new StringReader("Y");
        _console!.In = input;

        var program = new List<string> {
            "10 y=1: n=0",
            "20 input \"Enter (Y/N)\";a",
            "30 if a = 1 then 100",
            "40 print \"You entered 'NO'\"",
            "50 end",
            "100 print \"You entered 'YES'\""
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Enter (Y/N)?You entered 'YES'");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Handles_Indirect_References_On_Input2()
    {
        using var input = new StringReader("N");
        _console!.In = input;

        var program = new List<string> {
            "10 y=1: n=0",
            "20 input \"Enter (Y/N)\";a",
            "30 if a = 1 then 100",
            "40 print \"You entered 'NO'\"",
            "50 end",
            "100 print \"You entered 'YES'\""
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Enter (Y/N)?You entered 'NO'");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Handles_Indirect_References_On_Input3()
    {
        using var input = new StringReader("Z");
        _console!.In = input;

        var program = new List<string> {
            "10 y=2: n=1",
            "20 input \"Enter (Y/N)\";a",
            "30 if a = 2 then 100",
            "40 if a = 1 then 200",
            "50 print \"You didn't enter 'Y' or 'N'\"",
            "60 end",
            "100 print \"You entered 'YES'\"",
            "110 end",
            "200 print \"You entered 'NO'\"",
            "210 end"
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Enter (Y/N)?You didn't enter 'Y' or 'N'");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Handles_Extra_String_Variables()
    {
        var program = new List<string> {
            "10 C$=\"Chris\"",
            "20 print C$"

        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("Chris");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Handles_Extra_Array_Variables()
    {
        var program = new List<string> {
            "10 F(10) = 3.14159",
            "20 print F(10)"

        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 3.14159 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Executes_Read_Into_Array()
    {
        var program = new List<string> {
            "10 data 1,2,3,4,5",
            "20 for i = 1 to 5",
            "30 read a(i)",
            "40 next i",
            "50 for i = 1 to 5",
            "60 print a(i);",
            "70 next i",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be(" 1  2  3  4  5 ");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions1()
    {
        var program = new List<string> {
            "10 a=1:b=1",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("TRUE");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions2()
    {
        var program = new List<string> {
            "10 a=1:b=0",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("FALSE");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions3()
    {
        var program = new List<string> {
            "10 a=0:b=1",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("FALSE");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions4()
    {
        var program = new List<string> {
            "10 a=0:b=0",
            "20 if (a=1) * (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("FALSE");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions5()
    {
        var program = new List<string> {
            "10 a=1:b=1",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("TRUE");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions6()
    {
        var program = new List<string> {
            "10 a=1:b=0",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("TRUE");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions7()
    {
        var program = new List<string> {
            "10 a=0:b=1",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("TRUE");
        sr.ReadToEnd();
    }

    [TestMethod]
    public void Interpreter_Evaluates_Logical_Expressions8()
    {
        var program = new List<string> {
            "10 a=0:b=0",
            "20 if (a=1) + (b=1) then 100",
            "30 print \"FALSE\"",
            "40 end",
            "100 print \"TRUE\"",
        };
        RunProgram(program);

        using var sr = new StringReader(_sw.ToString());

        string? output = sr.ReadLine();
        output.Should().Be("FALSE");
        sr.ReadToEnd();
    }
}