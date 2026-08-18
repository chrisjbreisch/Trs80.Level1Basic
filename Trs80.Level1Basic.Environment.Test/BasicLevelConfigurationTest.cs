using System;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.Application;
using Trs80.Level1Basic.Common;

namespace Trs80.Level1Basic.Environment.Test;

[TestClass]
[DoNotParallelize]
public class BasicLevelConfigurationTest
{
    private const string BasicLevelEnvironmentVariable = "AppSettings__BasicLevel";

    [TestMethod]
    public void Bootstrapper_Defaults_To_Level_Two_When_BasicLevel_Is_Not_Configured()
    {
        string? originalValue = System.Environment.GetEnvironmentVariable(BasicLevelEnvironmentVariable);
        try
        {
            System.Environment.SetEnvironmentVariable(BasicLevelEnvironmentVariable, null);

            using var bootstrapper = new Bootstrapper();

            bootstrapper.AppSettings.BasicLevel.Should().Be(BasicLanguageLevel.Level2);
            bootstrapper.ScopedServiceProvider.GetRequiredService<BasicLanguageLevel>()
                .Should().Be(BasicLanguageLevel.Level2);
        }
        finally
        {
            System.Environment.SetEnvironmentVariable(BasicLevelEnvironmentVariable, originalValue);
        }
    }

    [TestMethod]
    public void Bootstrapper_Uses_Level_One_Environment_Override()
    {
        string? originalValue = System.Environment.GetEnvironmentVariable(BasicLevelEnvironmentVariable);
        try
        {
            System.Environment.SetEnvironmentVariable(BasicLevelEnvironmentVariable, "Level1");

            using var bootstrapper = new Bootstrapper();

            bootstrapper.AppSettings.BasicLevel.Should().Be(BasicLanguageLevel.Level1);
            bootstrapper.ScopedServiceProvider.GetRequiredService<BasicLanguageLevel>()
                .Should().Be(BasicLanguageLevel.Level1);
        }
        finally
        {
            System.Environment.SetEnvironmentVariable(BasicLevelEnvironmentVariable, originalValue);
        }
    }

    [TestMethod]
    public void Bootstrapper_Rejects_Unknown_BasicLevel()
    {
        string? originalValue = System.Environment.GetEnvironmentVariable(BasicLevelEnvironmentVariable);
        try
        {
            System.Environment.SetEnvironmentVariable(BasicLevelEnvironmentVariable, "Level3");

            Action createBootstrapper = () => new Bootstrapper().Dispose();

            createBootstrapper.Should().Throw<InvalidOperationException>();
        }
        finally
        {
            System.Environment.SetEnvironmentVariable(BasicLevelEnvironmentVariable, originalValue);
        }
    }

    [TestMethod]
    [DataRow(BasicLanguageLevel.Level1)]
    [DataRow(BasicLanguageLevel.Level2)]
    public void Bootstrapper_Can_Use_An_Explicit_Test_Profile(BasicLanguageLevel expectedLevel)
    {
        using var bootstrapper = new Bootstrapper(expectedLevel);

        bootstrapper.AppSettings.BasicLevel.Should().Be(expectedLevel);
        bootstrapper.ScopedServiceProvider.GetRequiredService<BasicLanguageLevel>()
            .Should().Be(expectedLevel);
    }
}
