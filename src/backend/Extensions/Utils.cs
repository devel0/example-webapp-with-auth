namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// states if given type is nullable of a enum
    /// </summary>
    public static bool IsNullableEnum(this Type type) =>
        type.IsGenericType &&
        type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
        type.GetGenericArguments()[0].IsEnum;

    /// <summary>
    /// states if given type implements given iType
    /// </summary>
    public static bool ImplementsInterface(this Type type, Type iType) =>
        type.GetInterfaces().Contains(iType);

    /// <summary>
    /// states if given type is a List of given listItemType
    /// </summary>
    /// <param name="type">Type to check></param>
    /// <param name="listItemType">Type of list template</param>    
    public static bool IsListOf(this Type type, Type listItemType)
    {
        var listItemTypeIsInterface = listItemType.IsInterface;
        var isList = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

        var matchesGivenListItemType = isList && (listItemTypeIsInterface ?
            type.GenericTypeArguments[0].ImplementsInterface(listItemType) :
            type.GenericTypeArguments[0] == listItemType);

        return isList && matchesGivenListItemType;
    }

    /// <summary>
    /// format full datetimespec w/zone (ie. 20250417T231031+02)
    /// </summary>
    public static string ToyyyyMMddTHHmmsszz(this DateTimeOffset timestamp) =>
        $"{timestamp:yyyyMMddTHHmmsszz}";

    /// <summary>
    /// format timezoned (ie. 2025/04/17 23:10:31 )
    /// </summary>
    public static string ToTimezonedDateAndTime(this DateTimeOffset timestamp, TimeZoneInfo timeZone)
    {
        var x = timestamp.ToOffset(timeZone.GetUtcOffset(timestamp));

        return
            $"{x.Year:D4}/{x.Month:D2}/{x.Day:D2} " +
            $"{x.Hour:D2}:{x.Minute:D2}:{x.Second:D2}"
            ;
    }

}

public static partial class Toolkit
{

    public static bool TypeIsReferenceOrNullable(this Type type) =>
        !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

    static Type IRecordWithGuidType = typeof(IRecordWithGuid);

    public static T CopyRecord<T>(T cur, T origItem, T updatedItem)
    {
        SearchAThing.Ext.Toolkit.CopyFrom(
            cur,
            updatedItem,
            prop =>
            {
                var res = false;

                if (prop.PropertyType.ImplementsInterface(IRecordWithGuidType))
                {
                    var updatedItemNest = prop.GetValue(updatedItem, null) as IRecordWithGuid;
                    var origItemNest = prop.GetValue(origItem, null) as IRecordWithGuid;

                    res = updatedItemNest?.Id != origItemNest?.Id;
                }

                else
                    res =
                        prop.GetValue(updatedItem, null)
                        !=
                        prop.GetValue(origItem, null);

                return res;
            });

        return cur;
    }

    public static string TruncateAtMaxLen(this string str, int maxLen, string suffix = "")
    {
        if (str.Length > maxLen)
            return str.Substring(0, maxLen - suffix.Length) + suffix;

        return str;
    }

}
