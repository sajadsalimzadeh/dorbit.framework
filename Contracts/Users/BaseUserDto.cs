﻿using System;
using System.Security.Claims;
using Dorbit.Framework.Contracts.Abstractions;

namespace Dorbit.Framework.Contracts.Users;

public class BaseUserDto : IUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public ClaimsPrincipal Claims { get; set; }
}