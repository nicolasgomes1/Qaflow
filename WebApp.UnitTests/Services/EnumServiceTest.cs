using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Services;

namespace WebApp.UnitTests.Services;

[TestClass]
[TestSubject(typeof(EnumService))]
public class EnumServiceTest
{
    
    private enum TestEnum
    {
        Alpha,
        Beta
    }

    [TestMethod]
    public void GetEnumValues()
    {
        var result = EnumService.GetEnumValues<TestEnum>();
        CollectionAssert.AreEqual(new List<TestEnum> { TestEnum.Alpha, TestEnum.Beta }, result);
    }

    [TestMethod]
    public void GetEnumFilteredValues()
    {
        var result = EnumService.GetEnumFilteredValues(TestEnum.Alpha);
        CollectionAssert.AreEqual(new List<TestEnum> { TestEnum.Beta }, result);
    }

    [TestMethod]
    public void GetSingleEnum()
    {
        var result = EnumService.GetSingleEnum(TestEnum.Alpha);
        Assert.AreEqual(TestEnum.Alpha, result);
    }
}