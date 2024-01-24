// <copyright file="CompanyNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    using System.Runtime.Serialization;

    [Serializable]
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

        protected CompanyNotFoundException(SerializationInfo info, StreamingContext context)
              : base(info, context)
        {
        }

        public static CompanyNotFoundException FromId(string erpId)
        {
            return new CompanyNotFoundException($"La company avec l\'id '{erpId}' n'a pas été trouvée dans le référentiel");
        }

        public static CompanyNotFoundException FromSiret(string siret)
        {
            return new CompanyNotFoundException($"La company avec le siret '{siret}' n'a pas été trouvée dans le référentiel");
        }
    }
}
