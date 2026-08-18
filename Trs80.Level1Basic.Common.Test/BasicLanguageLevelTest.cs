using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Common;

namespace Trs80.Level1Basic.Common.Test;

[TestClass]
public class BasicLanguageLevelTest
{
    [TestMethod]
    public void BasicLanguageLevel_Defines_Level_One_And_Level_Two()
    {
        BasicLanguageLevel.Level1.Should().NotBe(BasicLanguageLevel.Level2);
    }

    [TestMethod]
    public void AppSettings_Defaults_To_Level_Two()
    {
        var settings = new AppSettings();

        settings.BasicLevel.Should().Be(BasicLanguageLevel.Level2);
    }

    [TestMethod]
    public void AppSettings_Can_Select_Level_One()
    {
        var settings = new AppSettings
        {
            BasicLevel = BasicLanguageLevel.Level1
        };

        settings.BasicLevel.Should().Be(BasicLanguageLevel.Level1);
    }
}
