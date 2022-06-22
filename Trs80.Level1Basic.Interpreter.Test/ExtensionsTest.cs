using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class ExtensionsTest
{
    [TestMethod]
    public void Interpreter_Handles_Extra_String_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 C$=\"Chris\"",
            "20 print C$"

        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("CHRIS");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Handles_Extra_Array_Variables()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 F(10) = 3.14159",
            "20 print F(10)"

        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be(" 3.14159 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}