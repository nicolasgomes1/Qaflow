using JetBrains.Annotations;
using WebApp.Services;

namespace WebApp.UnitTests.Services;

[TestSubject(typeof(EnumService))]
public class EnumServiceTest
{
    
    private enum TestEnum
    {
        Alpha,
        Beta
    }

    [Fact]
    public void GetEnumValues()
    {
        var result = EnumService.GetEnumValues<TestEnum>();
        Assert.Equal(new List<TestEnum> { TestEnum.Alpha, TestEnum.Beta }, result);        
    }

    [Fact]
    public void GetEnumFilteredValues()
    {
        var result = EnumService.GetEnumFilteredValues(TestEnum.Alpha);
        Assert.Equal(new List<TestEnum> { TestEnum.Beta }, result);
        
    }

    [Fact]
    public void GetSingleEnum()
    {
        var result = EnumService.GetSingleEnum(TestEnum.Alpha);
        Assert.Equal(TestEnum.Alpha, result);
    }
}