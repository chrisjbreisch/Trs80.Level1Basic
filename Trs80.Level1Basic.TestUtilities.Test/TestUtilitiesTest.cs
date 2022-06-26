
using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.HostMachine;

namespace Trs80.Level1Basic.TestUtilities.Test;

[TestClass]
public class TestUtilitiesTest
{
    [TestMethod]
    public void FakeHost_Implements_EnableVirtualTerminal()
    {
        new FakeHost().EnableVirtualTerminal();
    }

    [TestMethod]
    public void FakeHost_Implements_GetCurrentConsoleFont()
    {
        HostFont font = new FakeHost().GetCurrentConsoleFont();
        font.Should().NotBeNull();
    }


    [TestMethod]
    public void FakeHost_Implements_SetCurrentConsoleFont()
    {
        var host = new FakeHost();
        HostFont font = host.GetCurrentConsoleFont();
        host.SetCurrentConsoleFont(font);
    }

    [TestMethod]
    public void FakeHost_Implements_GetCursorPosition()
    {
        var host = new FakeHost();
        (int left, int top) = host.GetCursorPosition();
        left.Should().Be(0);
        top.Should().Be(0);
    }

    [TestMethod]
    public void FakeHost_Implements_SetCursorPosition()
    {
        var host = new FakeHost();
        const int column = 20;
        const int row = 10;
        host.SetCursorPosition(column, row);
        (int left, int top) = host.GetCursorPosition();
        left.Should().Be(column);
        top.Should().Be(row);
    }

    [TestMethod]
    public void FakeHost_Implements_Clear()
    {
        var host = new FakeHost();
        const int column = 20;
        const int row = 10;
        host.SetCursorPosition(column, row);
        host.Clear();
        (int left, int top) = host.GetCursorPosition();
        left.Should().Be(0);
        top.Should().Be(0);
    }

    [TestMethod]
    public void FakeHost_Implements_ReadKey()
    {
        var host = new FakeHost();
        ConsoleKeyInfo key = host.ReadKey();
        key.Should().NotBeNull();
    }

    [TestMethod]
    public void FakeHost_Implements_SetWindowSize()
    {
        var host = new FakeHost();
        host.SetWindowSize(100, 100);
    }

    [TestMethod]
    public void FakeHost_Implements_SetBufferSize()
    {
        var host = new FakeHost();
        host.SetBufferSize(100, 100);
    }

    [TestMethod]
    public void FakeHost_Implements_Fill()
    {
        var host = new FakeHost();
        host.Fill(10, 10, 20, 20);
    }

    [TestMethod]
    public void FakeHost_Implements_Erase()
    {
        var host = new FakeHost();
        host.Erase(10, 10, 20, 20);
    }

    [TestMethod]
    public void FakeHost_Implements_GetFileNameForSave()
    {
        var host = new FakeHost();
        string fileName = host.GetFileNameForSave();
        fileName.Should().NotBeNull();
    }

    [TestMethod]
    public void FakeHost_Implements_GetFileNameForLoad()
    {
        var host = new FakeHost();
        string fileName = host.GetFileNameForLoad();
        fileName.Should().NotBeNull();
    }

    [TestMethod]
    public void FakeHost_Implements_Out()
    {
        var host = new FakeHost();
        TextWriter @out = host.Out;
        @out.Should().NotBeNull();
    }

    [TestMethod]
    public void FakeHost_Implements_In()
    {
        var host = new FakeHost();
        TextReader @in = host.In;
        @in.Should().NotBeNull();
    }

    [TestMethod]
    public void FakeHost_Implements_Error()
    {
        var host = new FakeHost();
        TextWriter error = host.Error;
        error.Should().NotBeNull();
    }
}