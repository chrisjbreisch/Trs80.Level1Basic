﻿using Trs80.Level1Basic.VirtualMachine.Scanner;

namespace Trs80.Level1Basic.CommandModels;

public class ScanModel
{
    public string SourceLine { get; set; } = string.Empty;
    public List<Token> Tokens { get; set; } = new();
}