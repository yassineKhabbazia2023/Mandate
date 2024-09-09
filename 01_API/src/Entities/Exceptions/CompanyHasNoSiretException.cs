// <copyright file="CustomBankCodeNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate;

public class CompanyHasNoSiretException : CustomException
{
    public CompanyHasNoSiretException()
    {
    }

    public CompanyHasNoSiretException(string message)
        : base(message)
    {
    }

    public CompanyHasNoSiretException(ExceptionType type, string message)
        : base(type, message)
    {
    }
}