﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Dorbit.Framework.Contracts.Results;

public class PagedListResult<T> : QueryResult<IEnumerable<T>>
{
    public int TotalCount { get; set; }

    public PagedListResult<TR> Select<TR>(Func<T, TR> func)
    {
        return new PagedListResult<TR>()
        {
            TotalCount = TotalCount,
            Data = Data.Select(func).ToList()
        };
    }

    public QueryResult<IEnumerable<T>> ToCommandResult()
    {
        return new QueryResult<IEnumerable<T>>()
        {
            Success = true,
            Data = Data,
        };
    }
}