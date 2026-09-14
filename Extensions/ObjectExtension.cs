using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace Dorbit.Framework.Extensions;

public static class ObjectExtension
{
    public static T Clone<T>(this T obj)
    {
        var type = obj.GetType();
        var tmp = Activator.CreateInstance<T>();
        foreach (var item in type.GetProperties())
        {
            item.SetValue(tmp, item.GetValue(obj));
        }

        return tmp;
    }

    public static T Overwrite<T>(this T obj1, T obj2, bool ignoreDefaults = false)
    {
        if (ignoreDefaults)
        {
            var obj = Activator.CreateInstance<T>();
            foreach (var property in obj1.GetType().GetProperties())
            {
                var value = property.GetValue(obj2);
                if (value.Equals(property.GetValue(obj))) continue;
                property.SetValue(obj1, value);
            }
        }
        else
        {
            foreach (var property in obj1.GetType().GetProperties()) property.SetValue(obj1, property.GetValue(obj2));
        }

        return obj1;
    }

    public static T Overwrite<T, TR>(this T obj1, TR obj2, bool ignoreDefaults = false) where T : TR
    {
        foreach (var property in obj2.GetType().GetProperties()) property.SetValue(obj1, property.GetValue(obj2));
        return obj1;
    }

    public static Dictionary<string, object> ToDictionary(this object o)
    {
        return o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(o, null));
    }

    public static T Patch<T>(this Dictionary<string, object> dict, T destination)
    {
        var destType = typeof(T);
        var destProperties = destType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var item in dict)
        {
            var destProperty = destProperties.FirstOrDefault(x => string.Equals(x.Name, item.Key, StringComparison.CurrentCultureIgnoreCase));
            if (destProperty is null) continue;
            if (item.Value is null)
            {
                destProperty.SetValue(destination, null);
                continue;
            }

            destProperty.SetValue(destination, item.Value.ConvertTo(destProperty.PropertyType));
        }

        return destination;
    }

    public static string ToJsonWeb(this object obj)
    {
        return JsonSerializer.Serialize(obj, JsonSerializerOptions.Web);
    }

    public static string ToXml(this object obj)
    {
        var type = obj.GetType();
        var sb = new StringBuilder();
        var xmlRootAttribute = type.GetCustomAttribute<XmlRootAttribute>();
        if (xmlRootAttribute is not null) sb.Append($"<{xmlRootAttribute.ElementName}>");
        foreach (var property in type.GetProperties())
        {
            var value = property.GetValue(obj);
            if (value == null) continue;
            var xmlElementAttribute = property.GetCustomAttribute<XmlElementAttribute>();
            var tagName = xmlElementAttribute?.ElementName ?? property.Name;

            if (property.PropertyType.IsNumeric() || property.PropertyType.IsString())
            {
                sb.Append($"<{tagName}>{value}</{tagName}>");
            }
            else if (value is bool)
            {
                sb.Append($"<{tagName}/>");
            }
            else if (value is IList enumerable)
            {
                var xmlArrayAttribute = property.GetCustomAttribute<XmlArrayAttribute>();
                var xmlArrayItemAttribute = property.GetCustomAttribute<XmlArrayItemAttribute>();

                var arrayTagName = xmlArrayAttribute?.ElementName ?? property.Name;
                var arrayItemTagName = xmlArrayItemAttribute?.ElementName ?? "item";
                if (enumerable.Count > 0)
                {
                    sb.Append($"<{arrayTagName}>");
                    foreach (var item in enumerable)
                    {
                        sb.Append($"<{arrayItemTagName}>");
                        sb.Append(item.ToXml());
                        sb.Append($"</{arrayItemTagName}>");
                    }

                    sb.Append($"</{arrayTagName}>");
                }
            }
            else
            {
                sb.Append($"<{tagName}>");
                sb.Append(value.ToXml());
                sb.Append($"</{tagName}>");
            }
        }

        if (xmlRootAttribute is not null) sb.Append($"</{xmlRootAttribute.ElementName}>");
        return sb.ToString();
    }
}