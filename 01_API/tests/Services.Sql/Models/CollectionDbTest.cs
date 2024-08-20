// <copyright file="CollectionDbTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    public class CollectionDbTest
    {
        [Fact]
        public void Defaults()
        {
            // Arrange & Act
            var entity = new CollectionDb();

            // Assert
            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(14);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(Guid.Empty);
            entity.Personal.Should().Be(null);
            entity.CompanyId.Should().Be(default);
            entity.Company.Should().Be(null);
            entity.Personal.Should().BeNull();
            entity.JeDeclareCollection.Should().BeNull();
            entity.Statuses.Should().BeNull();
            entity.BankCode.Should().BeNull();
            entity.Bank.Should().BeNull();
            entity.BranchCode.Should().BeNull();
            entity.AccountNumber.Should().BeNull();
            entity.CheckDigits.Should().BeNull();
            entity.LinkType.Should().BeNull();
            entity.RejectReason.Should().BeNull();
        }

        [Fact]
        public void Values()
        {
            // Arrange & Act
            PredictableGuid generator = new PredictableGuid();
            var comapnyId1 = 0;
            var collectionId1 = generator.NewGuid();

            var personal = new PersonalDb
            {
                CollectionId = collectionId1,
                CompanyId = comapnyId1,
                Title = "Mr.",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Street = "123 Main St",
                Complements = "Apt 4B",
                ZipCode = "12345",
                City = "Sample City",
                Country = "ExampleLand",
            };

            var company1 = new CompanyDb
            {
                Id = comapnyId1,
                Personal = personal,
                Name = "Microsoft",
                SiretNumber = "40902900600031",
                ErpId = "1000265308",
            };

            var refBankdb = new RefBankDb()
            {
                BankCode = "12345",
                BankName = "bn1",
                BankCommercialName = "bcn",
                BankCategory = "bca",
                BankGroup = "bg",
                IsJdcScrapable = true,
                IsJdcPartner = false,
                HasReleveAgreement = false,
                HasLiasseAgreement = null,
                AllowsDemat = true,
                JdcPartnership = (JdcPartnership)2,
                EbicsCardId = null,
            };

            var status = new StatusDb()
            {
                Id = new PredictableGuid(103).NewGuid(),
                CollectionId = new PredictableGuid(101).NewGuid(),
                StatusCode = -1,
                IsCurrent = true,
                StatusDate = new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc),
                MandateFile = null,
                CreatedBy = "created1",
            };

            var statusList = new List<StatusDb>() { status };

            var entity = new CollectionDb()
            {
                Id = collectionId1,
                CompanyId = comapnyId1,
                BankCode = "12345",
                BranchCode = "23456",
                AccountNumber = "12345678901",
                CheckDigits = "55",
                LinkType = 7,
                RejectReason = "reason1",
                Company = company1,
                Personal = personal,
                Bank = refBankdb,
                Statuses = statusList,
            };

            // Assert
            entity.Id.Should().Be(collectionId1);

            entity.Personal.CompanyId.Should().Be(comapnyId1);
            entity.Personal.CollectionId.Should().Be(collectionId1);
            entity.Personal.Title.Should().Be("Mr.");
            entity.Personal.FirstName.Should().Be("John");
            entity.Personal.LastName.Should().Be("Doe");
            entity.Personal.Email.Should().Be("john.doe@example.com");
            entity.Personal.Street.Should().Be("123 Main St");
            entity.Personal.Complements.Should().Be("Apt 4B");
            entity.Personal.ZipCode.Should().Be("12345");
            entity.Personal.City.Should().Be("Sample City");
            entity.Personal.Country.Should().Be("ExampleLand");

            entity.Bank.Should().BeEquivalentTo(refBankdb);
            entity.BankCode.Should().Be("12345");
            entity.BranchCode.Should().Be("23456");
            entity.Company.Should().Be(company1);
            entity.AccountNumber.Should().Be("12345678901");
            entity.CheckDigits.Should().Be("55");
            entity.LinkType.Should().Be(7);
            entity.RejectReason.Should().Be("reason1");
            entity.Statuses.Should().BeEquivalentTo(statusList);
        }
    }
}
