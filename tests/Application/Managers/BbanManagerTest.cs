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

            bbanManager.IsValid(new Bban("00000", "00000", "00000000000", "97", null)).Should().BeTrue();
            bbanManager.IsValid(new Bban("00000", "00000", "00000000000", "98", null)).Should().BeFalse();

            // source : https://fr.wikipedia.org/wiki/Cl%C3%A9_RIB
            bbanManager.IsValid(new Bban("12345", "12345", "1234567891A", "16", null)).Should().BeTrue();
            bbanManager.IsValid(new Bban("12345", "12345", "1234567891A", "17", null)).Should().BeFalse();

            // source : https://www.iban.fr/exemple.html
            bbanManager.IsValid(new Bban("30001", "00794", "12345678901", "85", null)).Should().BeTrue();
            bbanManager.IsValid(new Bban("30001", "00794", "12345678901", "86", null)).Should().BeFalse();

            // source : http://serge.mehl.free.fr/exos/cle_RIB.html
            bbanManager.IsValid(new Bban("15038", "01209", "1341531M025", "93", null)).Should().BeTrue();
            bbanManager.IsValid(new Bban("15038", "01209", "1341531M025", "94", null)).Should().BeFalse();
        }
    }
}
