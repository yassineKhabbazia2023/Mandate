// <copyright file="JdcCollecteConfigExistException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

[ExcludeFromCodeCoverage]
public class JdcCollecteConfigExistException : Exception
{
    public JdcCollecteConfigExistException()
    {
    }

    public JdcCollecteConfigExistException(string message)
        : base(message)
    {
    }

    public JdcCollecteConfigExistException(string message, Exception inner)
        : base(message, inner)
    {
    }
}