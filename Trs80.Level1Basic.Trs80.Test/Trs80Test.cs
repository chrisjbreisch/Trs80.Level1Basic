using System.IO;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Application;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.TestUtilities;
using Trs80.Level1Basic.VirtualMachine.Environment;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser;
using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.Trs80.Test;

[TestClass]
public class Trs80Test
{
    private IAppSettings? _appSettings;
    private ILoggerFactory? _loggerFactory;

    [TestInitialize]
    public void Initialize()
    {
        var bootstrapper = new Bootstrapper();
        _appSettings = bootstrapper.AppSettings;
        _loggerFactory = bootstrapper.LogFactory;
    }

    [TestMethod]
    public void Can_Replace_In_And_Out()
    {
        IBuiltinFunctions builtins = new BuiltinFunctions();
        var scanner = new Scanner(builtins);
        var parser = new Parser(builtins);
        IProgram program = new Program(scanner, parser);
        var trs80 = new VirtualMachine.Environment.Trs80(program, _appSettings, _loggerFactory, new FakeHost());

        const string input = "Hello, World!\r\n";
        var sw = new StringWriter();
        var sr = new StringReader(input);

        trs80.Out = sw;
        trs80.In = sr;

        string hello = trs80.ReadLine();
        trs80.WriteLine(hello);

        string output = sw.ToString();
        output.Should().Be(input);
    }
}