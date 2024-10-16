// <copyright file="BankHasNoJdcPartnershipException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

[ExcludeFromCodeCoverage]
public class BankHasNoJdcPartnershipException : Exception
{
    public BankHasNoJdcPartnershipException()
    {
    }

    public BankHasNoJdcPartnershipException(string message)
        : base(message)
    {
    }

    public BankHasNoJdcPartnershipException(string message, Exception inner)
        : base(message, inner)
    {
    }
}