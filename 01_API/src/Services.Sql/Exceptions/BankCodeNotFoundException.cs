// <copyright file="BankCodeNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

public class BankCodeNotFoundException : Exception
{
    public BankCodeNotFoundException()
    {
    }

    public BankCodeNotFoundException(string message)
        : base(message)
    {
    }

    public BankCodeNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public static BankCodeNotFoundException FromId(string bankCode)
    {
        return new BankCodeNotFoundException($"La banque avec le code '{bankCode}' n'a pas été trouvée dans le référentiel");
    }
}
