// <copyright file="TestHelper.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    internal static class TestHelper
    {
        public static Bank GetBank()
        {
            return new Bank("code", "name", "group", GetBankAgreement());
        }

        public static BankAgreement GetBankAgreement()
        {
            return new BankAgreement(true, true, true);
        }
    }
}
