// <copyright file="InactiveCompanyException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

public class InactiveCompanyException : Exception
{
    public InactiveCompanyException()
    {
    }

    public InactiveCompanyException(string message)
        : base(message)
    {
    }

    public InactiveCompanyException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public static InactiveCompanyException FromId(string erpId)
    {
        return new InactiveCompanyException($"La société dont l\'id est '{erpId}' n'est pas active");
    }
}