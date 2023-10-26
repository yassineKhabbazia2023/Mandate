// <copyright file="CompanyTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class CompanyTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new Company(
                new PredictableGuid().NewGuid(),
                "name",
                "siret",
                "ibsAccountNumber",
                "jdcDossierId",
                EntityFactory.Signatory,
                EntityFactory.Address);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(7);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(Guid.Parse("00000001-0000-0000-0000-000000000000"));
            entity.Name.Should().Be("name");
            entity.SiretNumber.Should().Be("siret");
            entity.ErpId.Should().Be("ibsAccountNumber");
            entity.BankServicesProviderId.Should().Be("jdcDossierId");
            entity.Signatory.Should().NotBeNull();
            entity.Address.Should().NotBeNull();
        }
    }
}
