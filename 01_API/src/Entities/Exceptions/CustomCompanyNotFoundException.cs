// <copyright file="CustomCompanyNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class CustomCompanyNotFoundException : CustomException
    {
        public CustomCompanyNotFoundException()
        {
        }

        public CustomCompanyNotFoundException(ExceptionType type, string message)
            : base(type, message)
        {
        }

        public static CustomCompanyNotFoundException FromId(ExceptionType type, string erpId)
        {
            return new CustomCompanyNotFoundException(type, $"La company avec l\'id '{erpId}' n'a pas été trouvée dans le référentiel");
        }

        public static CustomCompanyNotFoundException FromSiret(ExceptionType type, string siret)
        {
            return new CustomCompanyNotFoundException(type, $"La company avec le siret '{siret}' n'a pas été trouvée dans le référentiel");
        }

        public static CustomCompanyNotFoundException FromEprIdSiret(ExceptionType type, string erpId, string siret)
        {
            return new CustomCompanyNotFoundException(type, $"La company avec l\'erp id '{erpId}' et le siret '{siret}' n'a pas été trouvée dans le référentiel");
        }
    }
}
