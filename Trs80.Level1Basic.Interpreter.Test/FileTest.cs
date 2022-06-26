using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class FileTest
{
    [TestMethod]
    public void Interpreter_Can_Execute_Load()
    {
        using var controller = new TestController();

        controller.ExecuteLine("LOAD \"load.bas\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("Loaded \"load.bas\".");
        controller.ReadOutputLine().Should().Be(" 10 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Empty_Load()
    {
        using var controller = new TestController();

        controller.ExecuteLine("LOAD");
        controller.ExecuteLine("RUN");

        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Merge()
    {
        using var controller = new TestController();

        controller.ExecuteLine("LOAD \"load.bas\"");
        controller.ExecuteLine("MERGE \"merge.bas\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("Loaded \"load.bas\".");
        controller.ReadOutputLine().Should().Be("Merged \"merge.bas\".");
        controller.ReadOutputLine().Should().Be(" 20 ");
        controller.IsEndOfRun().Should().BeTrue();
    }


    [TestMethod]
    public void Interpreter_Can_Execute_Save()
    {
        using var controller = new TestController();

        controller.ExecuteLine("LOAD \"load.bas\"");
        controller.ExecuteLine("MERGE \"merge.bas\"");
        controller.ExecuteLine("30 A = A * 4");
        controller.ExecuteLine("SAVE \"save.bas\"");
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("LOAD \"save.bas\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("Loaded \"load.bas\".");
        controller.ReadOutputLine().Should().Be("Merged \"merge.bas\".");
        controller.ReadOutputLine().Should().Be("Saved \"save.bas\".");
        controller.ReadOutputLine().Should().Be("Loaded \"save.bas\".");
        controller.ReadOutputLine().Should().Be(" 80 ");
        controller.IsEndOfRun().Should().BeTrue();
    }
}