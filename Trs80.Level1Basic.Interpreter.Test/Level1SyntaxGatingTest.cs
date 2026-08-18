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

        controller.ReadOutputLine().Should().Be("?SN ERROR");
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

        controller.ReadOutputLine().Should().Be("?SN ERROR IN 10");
    }

    [TestMethod]
    public void Level1_Preserves_String_Type_Suffix()
    {
        using var controller = new TestController(BasicLanguageLevel.Level1);

        controller.ExecuteLine("10 A$=\"OK\"");

        controller.ReadOutputLine().Should().BeNull();
    }
}
