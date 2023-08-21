// <copyright file="BbanManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using System.Globalization;

    public class BbanManager : IBbanManager
    {
        public bool IsValid(string bban)
        {
            if (bban == null || bban.Length != 23)
            {
                return false;
            }

            var bankCode = bban[..5];
            var branchCode = bban[5..10];
            var accountNumber = bban[10..21];
            var checkDigits = bban[21..];

            var bankCodeNum = long.Parse(bankCode, CultureInfo.InvariantCulture);
            var branchCodeNum = long.Parse(branchCode, CultureInfo.InvariantCulture);
            var accountNumberNum = long.Parse(accountNumber.ReplaceFrenchBbanLetters(), CultureInfo.InvariantCulture);
            var checkDigitsNum = long.Parse(checkDigits, CultureInfo.InvariantCulture);

            var key = 97L - (((89L * bankCodeNum) + (15L * branchCodeNum) + (3L * accountNumberNum)) % 97L);

            return key == checkDigitsNum;
        }
    }
}
