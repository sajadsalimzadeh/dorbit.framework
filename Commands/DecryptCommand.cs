﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands.Abstractions;
using Dorbit.Framework.Contracts.Commands;
using Dorbit.Framework.Contracts.Cryptograpy;
using Dorbit.Framework.Extensions;

namespace Dorbit.Framework.Commands;

[ServiceRegister]
public class DecryptCommand : Command
{
    public override bool IsRoot { get; } = false;
    public override string Message => "Decrypt String";

    public override IEnumerable<CommandParameter> GetParameters(ICommandContext context)
    {
        yield return new CommandParameter("Input", "Input");
        yield return new CommandParameter("Key", "Key");
    }

    public override Task InvokeAsync(ICommandContext context)
    {
        var protectedProperty = new ProtectedProperty(context.GetArgAsString("Input")) { Algorithm = "SHA1" };
        context.Log($"{protectedProperty.GetDecryptedValue(context.GetArgAsString("Key").ToBytesUtf8())}\n");
        return Task.CompletedTask;
    }
}