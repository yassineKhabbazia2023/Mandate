// <copyright file="StringExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public static class StringExtensions
    {
        public static string ReplaceFrenchBbanLetters(this string source)
        {
            var mappingSrc = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var mappingDst = "012345678912345678912345678923456789";
            var result = source
                .ToUpper()
                .Select(x => mappingDst[mappingSrc.IndexOf(x)])
                .ToArray();
            return new string(result);
        }
    }
}
