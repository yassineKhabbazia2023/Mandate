// <copyright file="CompanyNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

public class CompanyNotFoundException : Exception
{
    public CompanyNotFoundException()
    {
    }

    public CompanyNotFoundException(string message)
        : base(message)
    {
    }

    public CompanyNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }

    public static CompanyNotFoundException FromId(string erpId)
    {
        return new CompanyNotFoundException($"La société avec l\'id '{erpId}' n'a pas été trouvée dans le référentiel");
    }

    public static CompanyNotFoundException FromSiret(string siret)
    {
        return new CompanyNotFoundException($"La société avec le siret '{siret}' n'a pas été trouvée dans le référentiel");
    }

    public static CompanyNotFoundException FromEprIdSiret(string erpId, string siret)
    {
        return new CompanyNotFoundException($"La société avec l\'erp id '{erpId}' et le siret '{siret}' n'a pas été trouvée dans le référentiel");
    }
}