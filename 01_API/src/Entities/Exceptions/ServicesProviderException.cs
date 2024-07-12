// <copyright file="ServicesProviderException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate;

public class ServicesProviderException : Exception
{
    public ServicesProviderException()
    {
    }

    public ServicesProviderException(string message)
        : base(message)
    {
    }

    public ServicesProviderException(string message, Exception inner)
        : base(message, inner)
    {
    }
}