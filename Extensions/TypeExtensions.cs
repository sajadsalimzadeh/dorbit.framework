﻿using System;
using System.Linq;
using System.Reflection;

namespace Dorbit.Framework.Extensions;

public static class TypeExtensions
{
    public static T GetStaticPropertyValue<T>(this Type type, string name) where T : class
    {
        return type
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(x => x.Name.Equals(name))?.GetValue(null) as T;
    }
}