// <copyright file="StringExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    public class StringExtensionsTest
    {
        [Fact]
        public void ReplaceFrenchBbanLetters()
        {
            "ajbkscltdmuenvfowgpxhqyirz0123456789".ReplaceFrenchBbanLetters().Should().Be("112223334445556667778889990123456789");
        }
    }
}
