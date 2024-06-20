using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;

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