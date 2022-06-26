
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trs80.Level1Basic.Common.Test;

class DisposeTest : DisposableBase {

}

[TestClass]
public class DisposableBaseTest
{
    [TestMethod]
    public void Can_Create_Disposable_Class()
    {
        var disposable = new DisposeTest();
        disposable.Should().NotBeNull();
    }

    [TestMethod]
    public void Can_Dispose_Disposable_Class()
    {
        var disposable = new DisposeTest();
        disposable.Dispose();
    }

    [TestMethod]
    public void Can_Dispose_Disposable_Class_Twice()
    {
        var disposable = new DisposeTest();
        disposable.Dispose();
        disposable.Dispose();
    }
}

