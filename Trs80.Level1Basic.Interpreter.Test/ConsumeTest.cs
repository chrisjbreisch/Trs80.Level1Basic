using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class ConsumeTest
{
    [TestMethod]
    public void Interpreter_Handles_Missing_Assignment()
    {
        using var controller = new TestController();
        var program = new List<string> {
            "10 FOR I 1 TO 10",
        };

        controller.RunProgram(program);

        controller.ReadOutputLine().Should().Be("WHAT?");
        controller.ReadErrorLine().Should().Be(" 10  FOR I ?1 TO 10");
        controller.ReadOutputLine();
        controller.IsEndOfRun().Should().BeTrue();
    }
}