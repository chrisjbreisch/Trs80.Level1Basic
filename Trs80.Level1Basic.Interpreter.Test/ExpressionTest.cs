using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class ExpressionTest
{
    [TestMethod]
    public void Interpreter_Can_Execute_Simple_Assignment()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Multiple_Assignments()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "15 i=7",
            "20 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_Simple_Math()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 j=4",
            "30 n=i*j",
            "40 print n"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 12 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Do_Harder_Math()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 c=25",
            "20 f=(9/5) * c + 32",
            "30 print f"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 77 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Addition()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 i=3",
            "20 i= i + 1",
            "30 print i"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Process_String_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 A$=\"Chris\"",
            "20 print \"Hello, \";A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HELLO, CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defint_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFINT A",
            "20 A = 3.9",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defsng_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSNG A",
            "20 A = 3.9",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.9 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defdbl_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFDBL A",
            "20 A = 3.9",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.9000000953674316 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Promotes_And_Narrows_Mixed_Scalar_Assignments()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFDBL A",
            "20 DEFSNG B",
            "30 A = 1.23456789012345",
            "40 B = A",
            "50 DEFDBL C",
            "60 C = B",
            "70 PRINT B;C"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 1.23457  1.2345678806304932 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Preserves_High_Precision_Literal_Assigned_Through_Defdbl()
    {
        using var controller = new TestController();

        controller.ExecuteLine("DEFDBL D");
        controller.ExecuteLine("D=1.23456789");
        controller.ExecuteLine("PRINT D");

        controller.ReadOutputLine().Should().Be(" 1.23456789 ");
    }

    [TestMethod]
    public void Interpreter_Can_Apply_Unary_Negation_To_Double()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFDBL A",
            "20 A = 3.9",
            "30 PRINT -A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-3.9000000953674316 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Preserves_Double_Precision_During_Division()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFDBL A-B",
            "20 A = 1",
            "30 B = 3",
            "40 PRINT A / B"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" .3333333 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Uses_Double_Numeric_Truthiness()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFDBL A",
            "20 A = 0.5",
            "30 IF A THEN PRINT \"TRUE\" : END",
            "40 PRINT \"FALSE\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Compares_Different_Numeric_Types_By_Value()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFDBL A",
            "20 A = 1",
            "30 IF A = 1 THEN PRINT \"TRUE\" : END",
            "40 PRINT \"FALSE\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("TRUE");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Compares_Strings_By_Value()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 PRINT \"YES\" > \"NO\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Orders_Different_Numeric_Types_By_Value()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 PRINT 1 < 1.5; 2! > 1#; 3# >= 3; 4! <= 4#"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("-1 -1 -1 -1 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defstr_Alias_After_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSTR A",
            "20 A = \"Chris\"",
            "30 PRINT A$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defstr_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSTR A",
            "20 A = \"Chris\"",
            "30 PRINT A"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defint_Range_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFINT A-Z",
            "20 A = 3.9",
            "30 B = 4.2",
            "40 PRINT A; B"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3  4 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defstr_Range_Declaration()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSTR A-C",
            "20 A = \"Chris\"",
            "30 B = \"Terry\"",
            "40 PRINT A; B"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CHRISTERRY");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Dim_Statement()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(10)",
            "20 A(3) = 7",
            "30 PRINT A(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_An_Implicit_Array()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 A(5) = 7",
            "20 PRINT A(5);A(6)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7  0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_An_Implicit_String_Array()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 A$(5) = \"YES\"",
            "20 PRINT A$(5);A$(6);\"!\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("YES!");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Rejects_Array_Index_Outside_Dimension()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(2)",
            "20 PRINT A(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HOW?");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Rejects_Redimensioning_An_Array()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(2)",
            "20 DIM A(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HOW?");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Rejects_Two_Subscripts_On_One_Dimensional_Array()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(2)",
            "20 PRINT A(1,1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HOW?");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Rejects_One_Subscript_On_Two_Dimensional_Array()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(2,2)",
            "20 PRINT A(1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HOW?");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Three_Dimensional_Array()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(2,2,2)",
            "20 A(1,1,1) = 7",
            "30 PRINT A(1,1,1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Initializes_Three_Dimensional_String_Array_Elements_To_Empty()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A$(2,2,2)",
            "20 PRINT A$(1,1,1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().BeEmpty();
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Clear_Resets_Three_Dimensional_Array_Values()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(2,2,2)",
            "20 A(1,1,1)=7",
            "30 CLEAR",
            "40 PRINT A(1,1,1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Run_Dimensioned_Array_Program_Twice()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(2)",
            "20 A(0)=7",
            "30 PRINT A(0)"
        };

        controller.ExecuteLine("NEW");
        controller.ExecuteStatements(program);
        controller.ExecuteLine("RUN");
        controller.ExecuteLine("RUN");

        int printedValues = 0;
        while (printedValues < 2)
        {
            if (controller.ReadOutputLine() == " 7 ")
                printedValues++;
        }

        printedValues.Should().Be(2);
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Clear_Resets_Two_Dimensional_Array_Values()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(2,2)",
            "20 A(1,1) = 7",
            "30 CLEAR",
            "40 PRINT A(1,1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Rejects_Negative_Array_Index()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 PRINT A(-1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("HOW?");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defint_Declaration_For_Array_Elements()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFINT A",
            "20 DIM A(10)",
            "30 A(3) = 3.9",
            "40 PRINT A(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Defstr_Declaration_For_String_Array_Elements()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSTR A",
            "20 DIM A$(10)",
            "30 A$(3) = \"Chris\"",
            "40 PRINT A$(3)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Array_Integer_Suffix_Overrides_Defstr()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFSTR A",
            "20 DIM A%(2)",
            "30 A%(1) = 3.9",
            "40 PRINT A%(1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Array_String_Suffix_Overrides_Defint()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DEFINT A",
            "20 DIM A$(2)",
            "30 A$(1) = \"YES\"",
            "40 PRINT A$(1)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("YES");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Initializes_String_Array_Elements_To_Empty()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A$(2)",
            "20 PRINT A$(0); \"X\""
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("X");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Use_Multidimensional_Array_Statement()
    {
        using var controller = new TestController();
        var program = new List<string>
        {
            "10 DIM A(10,10)",
            "20 A(3,4) = 7",
            "30 PRINT A(3,4)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 7 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}