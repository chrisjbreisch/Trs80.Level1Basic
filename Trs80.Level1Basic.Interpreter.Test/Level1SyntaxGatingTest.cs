using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Interpreter.Test;

[TestClass]
public class Level1SyntaxGatingTest
{
    [TestMethod]
    public void Level1_Rejects_Defint_Declaration()
    {
        using var controller = new TestController(BasicLanguageLevel.Level1);

        controller.ExecuteLine("DEFINT A");

        controller.ReadOutputLine().Should().Be("WHAT?");
    }

    [TestMethod]
    public void Level2_Accepts_Defint_Declaration()
    {
        using var controller = new TestController(BasicLanguageLevel.Level2);

        controller.ExecuteLine("DEFINT A");

        controller.ReadOutputLine().Should().BeNull();
    }

    [TestMethod]
    [DataRow("10 A%=5")]
    [DataRow("10 A!=5")]
    [DataRow("10 A#=5")]
    public void Level1_Rejects_Numeric_Type_Suffixes(string source)
    {
        using var controller = new TestController(BasicLanguageLevel.Level1);

        controller.ExecuteLine(source);

        controller.ReadOutputLine().Should().Be("WHAT?");
    }

    [TestMethod]
    public void Level1_Preserves_String_Type_Suffix()
    {
        using var controller = new TestController(BasicLanguageLevel.Level1);

        controller.ExecuteLine("10 A$=\"OK\"");

        controller.ReadOutputLine().Should().BeNull();
    }

    [TestMethod]
    [DataRow("10 PRINT 2^3")]
    [DataRow("10 PRINT 1D+2")]
    [DataRow("10 DIM A(1,1)")]
    [DataRow("10 A$=\"ABC\":MID$(A$,2,1)=\"X\"")]
    [DataRow("10 BEEP")]
    [DataRow("10 OUT 1,2")]
    [DataRow("10 WAIT 1,2")]
    [DataRow("10 LPRINT \"X\"")]
    [DataRow("10 LLIST")]
    [DataRow("10 PRINT AT 1,\"X\"")]
    [DataRow("10 PRINT CVI(\"AB\")")]
    public void Level1_Rejects_Level2_Only_Syntax(string source)
    {
        using var controller = new TestController(BasicLanguageLevel.Level1);

        controller.ExecuteLine(source);

        controller.ReadOutputLine().Should().Be("WHAT?");
    }

    [TestMethod]
    public void Level2_Accepts_Level2_Only_Syntax()
    {
        using var controller = new TestController(BasicLanguageLevel.Level2);

        controller.ExecuteLine("10 PRINT 2^3");
        controller.ExecuteLine("20 PRINT 1D+2");
        controller.ExecuteLine("30 DIM A(1,1)");
        controller.ExecuteLine("40 A$=\"ABC\":MID$(A$,2,1)=\"X\"");

        controller.ReadOutputLine().Should().BeNull();
    }

    [TestMethod]
    public void Level1_Uses_Integer_Default_For_Unsuffixed_Variables()
    {
        using var controller = new TestController(BasicLanguageLevel.Level1);
        controller.ExecuteLine("A=3.9");
        controller.ExecuteLine("PRINT A");

        controller.ReadOutputLine().Should().Be(" 3 ");
    }

    [TestMethod]
    public void Level2_Uses_Single_Default_For_Unsuffixed_Variables()
    {
        using var controller = new TestController(BasicLanguageLevel.Level2);
        controller.ExecuteLine("A=3.9");
        controller.ExecuteLine("PRINT A");

        controller.ReadOutputLine().Should().Be(" 3.9 ");
    }
}
