// <copyright file="RibIdEmptyOrNullException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate;

public class RibIdEmptyOrNullException : Exception
{
    public RibIdEmptyOrNullException()
    {
    }

    public RibIdEmptyOrNullException(string message)
        : base(message)
    {
    }

    public RibIdEmptyOrNullException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
