using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dorbit.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceRegisterAttribute : Attribute
{
    public int Order { get; set; } = 0;
    public bool Recursive { get; set; } = true;
    public int RecursiveLevelCount { get; set; } = 100;
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;
}