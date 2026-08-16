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
}
