using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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
    public void Load_Clears_The_Stop_Continuation_Point()
    {
        using var controller = new TestController();
        controller.RunProgram(new List<string> {
            "10 PRINT \"OLD\"",
            "20 STOP",
            "30 PRINT \"STALE\""
        });

        controller.ExecuteLine("LOAD \"load.bas\"");
        controller.ExecuteLine("CONT");

        controller.ReadOutputLine().Should().Be("OLD");
        controller.ReadOutputLine().Should().Be("BREAK IN 20");
        controller.ReadOutputLine().Should().Be("READY");
        controller.ReadOutputLine().Should().Be("Loaded \"load.bas\".");
        controller.ReadOutputLine().Should().BeEmpty();
        controller.ReadOutputLine().Should().Be("READY");
        controller.ReadOutputLine().Should().BeNull();
    }

    [TestMethod]
    public void Load_Clears_On_Error_State()
    {
        using var controller = new TestController();
        controller.ExecuteLine("80 PRINT \"STALE HANDLER\"");
        controller.ExecuteLine("ON ERROR GO TO 80");
        controller.ExecuteLine("LOAD \"load.bas\"");
        controller.ExecuteLine("S=1/A");

        controller.ReadOutputLine().Should().Be("Loaded \"load.bas\".");
        controller.ReadOutputLine().Should().Be("?/0 ERROR");
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
    public void Cancelling_Load_Preserves_The_Current_Program()
    {
        using var controller = new TestController();
        controller.ExecuteLine("10 PRINT \"KEPT\"");

        controller.ExecuteLine("LOAD");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("KEPT");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Uses_File_Open_Dialog_Path_For_Bare_Load()
    {
        using var controller = new TestController();
        controller.Host.FileNameForLoad = "load.bas";

        controller.ExecuteLine("LOAD");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("Loaded \"load.bas\".");
        controller.ReadOutputLine().Should().Be(" 10 ");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Can_Execute_Cassette_Aliases_Through_File_Commands()
    {
        using var controller = new TestController();

        controller.ExecuteLine("CLOAD \"load.bas\"");
        controller.ExecuteLine("CSAVE \"csave.bas\"");
        controller.ExecuteLine("NEW");
        controller.ExecuteLine("CLOAD \"csave.bas\"");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("Loaded \"load.bas\".");
        controller.ReadOutputLine().Should().Be("Saved \"csave.bas\".");
        controller.ReadOutputLine().Should().Be("Loaded \"csave.bas\".");
        controller.ReadOutputLine().Should().Be(" 10 ");
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

    [TestMethod]
    public void Cancelling_Save_Preserves_The_Current_Program_Without_A_Success_Message()
    {
        using var controller = new TestController();
        controller.ExecuteLine("10 PRINT \"KEPT\"");

        controller.ExecuteLine("SAVE");
        controller.ExecuteLine("RUN");

        controller.ReadOutputLine().Should().Be("KEPT");
        controller.IsEndOfRun().Should().BeTrue();
    }

    [TestMethod]
    public void Interpreter_Uses_File_Save_Dialog_Path_For_Bare_Save()
    {
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.bas");

        try
        {
            using var controller = new TestController();
            controller.Host.FileNameForSave = path;
            controller.ExecuteLine("10 PRINT 10");

            controller.ExecuteLine("SAVE");

            File.Exists(path).Should().BeTrue();
            controller.ReadOutputLine().Should().Be($"Saved \"{path}\".");
        }
        finally
        {
            File.Delete(path);
        }
    }
}