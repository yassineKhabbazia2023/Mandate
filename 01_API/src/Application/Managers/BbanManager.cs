// <copyright file="BbanManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using System.Globalization;

    public class BbanManager : IBbanManager
    {
        public bool IsValid(Bban bban)
        {
            if (bban == null)
            {
                return false;
            }

            try
            {
                var bankCodeNum = long.Parse(bban.BankCode, CultureInfo.InvariantCulture);
                var branchCodeNum = long.Parse(bban.BranchCode, CultureInfo.InvariantCulture);
                var accountNumberNum = long.Parse(bban.AccountNumber.ReplaceFrenchBbanLetters(), CultureInfo.InvariantCulture);
                var checkDigitsNum = long.Parse(bban.CheckDigits, CultureInfo.InvariantCulture);

                var key = 97L - (((89L * bankCodeNum) + (15L * branchCodeNum) + (3L * accountNumberNum)) % 97L);

                return key == checkDigitsNum;
            }
            catch
            {
                return false;
            }
        }
    }
}
