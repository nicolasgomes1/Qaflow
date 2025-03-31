namespace WebApp.Services;

public static class EnumService
{
    public static List<T> GetEnumValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    }

    public static List<T> GetEnumFilteredValues<T>(params T[] excludedValues)
    {
        var enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToList();
        return enumValues.Except(excludedValues).ToList();
    }

    public static T GetSingleEnum<T>(T value)
    {
        return value;
    }
}