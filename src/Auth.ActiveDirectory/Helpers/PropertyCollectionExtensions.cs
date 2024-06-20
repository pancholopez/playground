using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using System.Text.RegularExpressions;

namespace Auth.ActiveDirectory.Helpers;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal static class PropertyCollectionExtensions
{
    public static string GetValueOrDefault(this ResultPropertyCollection properties,
        string propertyName, string defaultValue = "N/A")
    {
        if (!properties.Contains(propertyName)) return defaultValue;

        var propertyValue = properties[propertyName][0];
        if (propertyValue is byte[] bytes) // if the value is a GUID
        {
            return new Guid(bytes).ToString();
        }

        return propertyValue.ToString()!;
    }

    public static ICollection<string> GetCollectionOrDefault(this ResultPropertyCollection properties,
        string propertyName) => properties.Contains(propertyName)
        ? properties[propertyName].OfType<string>().ToList()
        : [];

    public static string GetValueOrDefault(this PropertyCollection properties,
        string propertyName, string defaultValue = "N/A")
    {
        if (!properties.Contains(propertyName)) return defaultValue;

        var propertyValue = properties[propertyName][0];
        if (propertyValue is byte[] bytes) // if the value is a GUID
        {
            return new Guid(bytes).ToString();
        }

        return propertyValue?.ToString() ?? string.Empty;
    }
}

public static class UniqueHelper
{
    public static string GetUniqueUsername(string name, List<string> existingNames)
    {
        if (!existingNames.Contains(name))
        {
            return name;
        }

        var regex = new Regex(Regex.Escape(name) + @"(\d+)$");
        var maxNumber = existingNames
            .Select(n => regex.Match(n))
            .Where(m => m.Success)
            .Select(m => int.Parse(m.Groups[1].Value))
            .DefaultIfEmpty(0)
            .Max();

        return $"{name}{maxNumber + 1}";
    }
}