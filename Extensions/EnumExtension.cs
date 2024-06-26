using System;
using System.ComponentModel;
using System.Reflection;

namespace Dorbit.Framework.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this Enum e)
    {
        var type = e.GetType();
        var memInfo = type.GetMember(e.ToString());
        if (memInfo.Length > 0)
        {
            var attribute = memInfo[0].GetCustomAttribute(typeof(DescriptionAttribute));
            return ((DescriptionAttribute)attribute)?.Description;
        }

        return null;
    }
}