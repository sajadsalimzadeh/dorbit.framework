﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands.Abstractions;
using Dorbit.Framework.Contracts.Commands;
using Dorbit.Framework.Utils.Cryptography;

namespace Dorbit.Framework.Commands;

[ServiceRegister]
public class EncryptCommand : Command
{
    public override bool IsRoot { get; } = false;
    public override string Message => "Encrypt String";

    public override IEnumerable<CommandParameter> GetParameters(ICommandContext context)
    {
        yield return new CommandParameter("Input", "Input");
        yield return new CommandParameter("Key", "Key");
    }

    public override Task InvokeAsync(ICommandContext context)
    {
        var cypherText = new Aes(context.Arguments["Key"].ToString()).Encrypt(context.Arguments["Input"].ToString());
        context.Log($"{Convert.ToBase64String(cypherText)}\n");
        return Task.CompletedTask;
    }
}