// <copyright file="RefBankDbTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    public class RefBankDbTest
    {
        [Fact]
        public void Defaults()
        {
            var entity = new RefBankDb();

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(11);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(Guid.Empty);
            entity.BankCode.Should().Be(string.Empty);
            entity.BankName.Should().BeNull();
            entity.BankCommercialName.Should().BeNull();
            entity.BankCategory.Should().BeNull();
            entity.BankGroup.Should().BeNull();
            entity.IsJdcScrapable.Should().BeFalse();
            entity.IsJdcPartner.Should().BeFalse();
            entity.HasReleveAgreement.Should().BeNull();
            entity.HasLiasseAgreement.Should().BeNull();
            entity.AllowsDemat.Should().BeNull();
        }

        [Fact]
        public void Values()
        {
            var entity = new RefBankDb()
            {
                Id = new PredictableGuid().NewGuid(),
                BankCode = "bc",
                BankName = "bn",
                BankCommercialName = "bcn",
                BankCategory = "bca",
                BankGroup = "bg",
                IsJdcScrapable = true,
                IsJdcPartner = true,
                HasReleveAgreement = false,
                HasLiasseAgreement = false,
                AllowsDemat = false,
            };

            entity.Id.Should().Be(Guid.Parse("00000001-0000-0000-0000-000000000000"));
            entity.BankCode.Should().Be("bc");
            entity.BankName.Should().Be("bn");
            entity.BankCommercialName.Should().Be("bcn");
            entity.BankCategory.Should().Be("bca");
            entity.BankGroup.Should().Be("bg");
            entity.IsJdcScrapable.Should().BeTrue();
            entity.IsJdcPartner.Should().BeTrue();
            entity.HasReleveAgreement.Should().BeFalse();
            entity.HasLiasseAgreement.Should().BeFalse();
            entity.AllowsDemat.Should().BeFalse();
        }
    }
}
