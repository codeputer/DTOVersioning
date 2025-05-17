namespace BlazorApp1.Client.Utilities.String;

public static class ParameterMarkers
{
    public static string ParamMarkers(this object? pObject, bool pTrim = false)
    {
        return (pObject?.ToString()).ParamMarkers(pTrim);
    }

    public static string ParamMarkers(this string? pString, bool pTrim = false)
    {
        if (string.IsNullOrEmpty(pString))
        {
            return " >NullValue< ";
        }

        return pTrim ? $" >{pString.Trim()}< " : $" >{pString}< ";
    }
}
