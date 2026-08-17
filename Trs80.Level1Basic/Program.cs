using System;
using System.Diagnostics;
using System.IO;

using Trs80.Level1Basic.Application;

namespace Trs80.Level1Basic;

internal class Program
{
    private const string ConhostLaunchMarker = "TRS80_LEVEL1BASIC_CONHOST";

    [STAThread]
    private static void Main(string[] args)
    {
        if (ShouldLaunchInConhost() && LaunchInConhost(args)) return;

        new ConsoleApp().Run("Interpreter.json", "Interpreter");
    }

    private static bool ShouldLaunchInConhost()
    {
        return OperatingSystem.IsWindows()
            && !string.Equals(Environment.GetEnvironmentVariable(ConhostLaunchMarker), "1", StringComparison.Ordinal)
            && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WT_SESSION"));
    }

    private static bool LaunchInConhost(string[] args)
    {
        string processPath = Environment.ProcessPath ?? throw new InvalidOperationException("The application path is unavailable.");
        var startInfo = new ProcessStartInfo
        {
            FileName = "conhost.exe",
            WorkingDirectory = Environment.CurrentDirectory,
            UseShellExecute = false
        };

        if (Path.GetFileNameWithoutExtension(processPath).Equals("dotnet", StringComparison.OrdinalIgnoreCase))
        {
            startInfo.ArgumentList.Add(typeof(Program).Assembly.Location);
        }
        else
        {
            startInfo.ArgumentList.Add(processPath);
        }

        foreach (string argument in args)
            startInfo.ArgumentList.Add(argument);

        startInfo.Environment[ConhostLaunchMarker] = "1";
        Process.Start(startInfo);
        return true;
    }
}