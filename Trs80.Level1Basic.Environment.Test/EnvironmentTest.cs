using System.Collections.Generic;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Trs80.Level1Basic.TestUtilities;

namespace Trs80.Level1Basic.Environment.Test
{
    [TestClass]
    public class EnvironmentTest
    {
        private readonly List<string> _program = new (){
            "10 i = 3",
            "20 i = i * 2",
            "30 print i"
        };

        [TestMethod]
        public void Environment_Can_Execute_Baseline_Program()
        {
            using var controller = new TestController();

            controller.RunProgram(_program);

            controller.ReadOutputLine().Should().Be(" 6 ");
            controller.IsEndOfRun().Should().BeTrue();
        }

        [TestMethod]
        public void Environment_Can_Insert_Line_In_The_Middle()
        {
            using var controller = new TestController();

            controller.ExecuteLine("new");
            controller.ExecuteStatements(_program);

            controller.ExecuteLine("25 i = i + 7");
            controller.ExecuteLine("run");

            controller.ReadOutputLine().Should().Be(" 13 ");
            controller.IsEndOfRun().Should().BeTrue();
        }

        [TestMethod]
        public void Environment_Can_Replace_Line()
        {
            using var controller = new TestController();

            controller.ExecuteLine("new");
            controller.ExecuteStatements(_program);

            controller.ExecuteLine("20 i = i + 7");
            controller.ExecuteLine("run");

            controller.ReadOutputLine().Should().Be(" 10 ");
            controller.IsEndOfRun().Should().BeTrue();
        }

        [TestMethod]
        public void Environment_Can_Delete_Line()
        {
            using var controller = new TestController();

            controller.ExecuteLine("new");
            controller.ExecuteStatements(_program);

            controller.ExecuteLine("20");
            controller.ExecuteLine("run");
            controller.ExecuteLine("list");

            controller.ReadOutputLine().Should().Be(" 3 ");
            controller.ReadOutputLine().Should().Be("");
            controller.ReadOutputLine().Should().Be("READY");
            controller.ReadOutputLine().Should().Be(" 10  I = 3");
            controller.ReadOutputLine().Should().Be(" 30  PRINT I");
            controller.ReadOutputLine().Should().BeNull();
        }
    }
}