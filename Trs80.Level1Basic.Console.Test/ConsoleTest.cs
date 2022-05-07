using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trs80.Level1Basic.Application;
using Trs80.Level1Basic.Common;
using FluentAssertions;

namespace Trs80.Level1Basic.Console.Test;

[TestClass]
public class ConsoleTest
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
        string input = "Hello, World!\r\n";
        IConsole console = new Console(_appSettings, _loggerFactory, false);
        var sw = new StringWriter();
        var sr = new StringReader(input);

        console.Out = sw;
        console.In = sr;

        string hello = console.ReadLine();
        console.WriteLine(hello);

        string output = sw.ToString();
        output.Should().Be(input);
    }
}