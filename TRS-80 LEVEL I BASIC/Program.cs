﻿using Trs80.Level1Basic.Application;

namespace Trs80.Level1Basic;

internal class Program
{
    private static void Main()
    {
        new ConsoleApp().Run("Interpreter.json", "Interpreter");
    }
}