// <copyright file="BbanManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    public class BbanManagerTest
    {
        [Fact]
        public void IsValid()
        {
            var bbanManager = new BbanManager();

            bbanManager.IsValid(null!).Should().BeFalse();
            bbanManager.IsValid("x").Should().BeFalse();

            bbanManager.IsValid("00000000000000000000097").Should().BeTrue();
            bbanManager.IsValid("00000000000000000000098").Should().BeFalse();

            // source : https://fr.wikipedia.org/wiki/Cl%C3%A9_RIB
            bbanManager.IsValid("12345123451234567891A16").Should().BeTrue();
            bbanManager.IsValid("12345123451234567891A17").Should().BeFalse();

            // source : https://www.iban.fr/exemple.html
            bbanManager.IsValid("30001007941234567890185").Should().BeTrue();
            bbanManager.IsValid("30001007941234567890186").Should().BeFalse();

            // source : http://serge.mehl.free.fr/exos/cle_RIB.html
            bbanManager.IsValid("15038012091341531M02593").Should().BeTrue();
            bbanManager.IsValid("15038012091341531M02594").Should().BeFalse();
        }

    }
}
