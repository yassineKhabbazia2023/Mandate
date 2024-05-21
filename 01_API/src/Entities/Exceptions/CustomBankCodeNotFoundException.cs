// <copyright file="BankCodeNotFoundException.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class CustomBankCodeNotFoundException : CustomException
    {
        public CustomBankCodeNotFoundException()
        {
        }

        public CustomBankCodeNotFoundException(string message)
            : base(message)
        {
        }

        public CustomBankCodeNotFoundException(ExceptionType type, string message)
            : base(type, message)
        {
        }

        public static CustomBankCodeNotFoundException FromId(string bankCode)
        {
            return new CustomBankCodeNotFoundException($"La banque avec le code '{bankCode}' n'a pas été trouvée dans le référentiel");
        }
    }
}
