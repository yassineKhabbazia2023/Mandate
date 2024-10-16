// <copyright file="InaccessibleCompanyException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

public class InaccessibleCompanyException : Exception
{
    public InaccessibleCompanyException()
    {
    }

    public InaccessibleCompanyException(string message)
        : base(message)
    {
    }

    public InaccessibleCompanyException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public static InaccessibleCompanyException FromId(string erpId)
    {
        return new InaccessibleCompanyException($"La société dont l\'id est '{erpId}' n'est pas accessible pour le collaborateur");
    }
}