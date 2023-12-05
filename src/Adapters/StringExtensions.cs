// <copyright file="StringExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    public static class StringExtensions
    {
        public static string Extract(this string input, int index, int len)
        {
            if (string.IsNullOrEmpty(input) || input.Length < len)
            {
                return input;
            }

            return input.Substring(index, len);
        }
    }
}
