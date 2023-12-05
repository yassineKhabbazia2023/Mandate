// <copyright file="CollectionCreationCommandTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class CollectionCreationCommandTest
    {
        [Fact]
        public void Constructor()
        {
            var signatory = new Signatory(
               "Mme",
               "First",
               "Last",
               "first.last@outlook.com");
            var address = new Address(
                "street",
                "complements",
                "75001",
                "Paris",
                "France");
            var bban = new Bban(
                "12345",
                "54321",
                "12345678901",
                "01");
            var entity = new CollectionCreationCommand("e", signatory, address, bban);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(4);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.ErpId.Should().Be("e");
            entity.Signatory.Should().Be(signatory);
            entity.Address.Should().Be(address);
            entity.Bban.Should().Be(bban);
        }

        [Fact]
        public void Serialization()
        {
            var signatory = new Signatory(
               "Mme",
               "First",
               "Last",
               "first.last@outlook.com");
            var address = new Address(
                "street",
                "complements",
                "75001",
                "Paris",
                "France");
            var bban = new Bban(
                "12345",
                "54321",
                "12345678901",
                "01");
            var entity = new CollectionCreationCommand("e", signatory, address, bban);

            entity.Should().BeJsonSerializableTo(new
            {
                erpId = "e",
                signatory = new
                {
                    title = "Mme",
                    firstName = "First",
                    lastName = "Last",
                    email = "first.last@outlook.com",
                },
                address = new
                {
                    street = "street",
                    addressComplement = "complements",
                    zipCode = "75001",
                    city = "Paris",
                    country = "France",
                },
                bban = new
                {
                    bankCode = "12345",
                    branchCode = "54321",
                    accountNumber = "12345678901",
                    checkDigits = "01",
                },
            });
        }
    }
}
