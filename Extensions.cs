public static class ExtensionMethods
{
    public static string Capitalize(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        return char.ToUpper(str[0]) + str.Substring(1);
    }
    public static string RemoveFileExt(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        var parts = str.Split('.');
        return string.Join("", parts[0..^1]);
    }
}