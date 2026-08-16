using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class Level2CompatibilityTest
{
    [TestMethod]
    public void Hardware_Statements_Preserve_Graphics_And_Memory_Behavior()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 SET 23,20",
            "15 RESET 23,20",
            "20 POKE 100,65",
            "30 PRINT POINT(23,20);PEEK(100)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 0  65 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Typed_Array_And_Mid_Assignment_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 DEFINT A",
            "20 DIM A(2)",
            "30 A(1)=42",
            "40 B$=\"HELLO\"",
            "50 MID$(B$,2,2)=\"AI\"",
            "60 PRINT A(1);B$"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 42 HAILO");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Binary_Conversion_Compatibility_Program()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 PRINT CVI(\"AB\")",
            "20 PRINT MKI$(16961)"
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 16961 ");
        controller.ReadOutputLine().Should().Be("AB");
        controller.IsEndOfRun().Should().BeTrue();
    }
}
