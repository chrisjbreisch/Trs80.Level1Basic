// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", 
    Justification = "Too much of a pain if they later need to not be static. Not worth the minimal performance gain.", 
    Scope = "member", Target = "~M:Trs80.Level1Basic.Console.Console.SetBufferSize(System.Int32,System.Int32)")]
[assembly: SuppressMessage("Usage", "CA2254:Template should be a static expression", 
    Justification = "Don't log all that much to be worth the effort required to change to the new faster logger", 
    Scope = "member", Target = "~M:Trs80.Level1Basic.Console.Console.InitializeWindowSettings")]
