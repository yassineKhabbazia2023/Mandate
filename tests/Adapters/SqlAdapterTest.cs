// <copyright file="SqlAdapterTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests;

    public class SqlAdapterTest
    {
        [Fact]
        public async Task GetAllCollections()
        {
            var status = EntityDbFactory.StatusDb;
            status.RefStatusCode = EntityDbFactory.RefStatusCodeDb;
            var coll = EntityDbFactory.CollectionDb;
            coll.Company = EntityDbFactory.CompanyDb;
            coll.Bank = EntityDbFactory.RefBankDb;
            coll.Statuses = new List<StatusDb>() { status };
            (List<CollectionDb>, int) tuple = (new List<CollectionDb>() { coll }, 1);

            var mandateRepository = new Mock<IMandateRepository>(MockBehavior.Strict);
            mandateRepository.Setup(r => r.SearchCollectionsAsync(It.IsAny<Sql.CollectionQuery>()))
                .ReturnsAsync(tuple)
                .Verifiable();

            SqlAdapter adapter = new SqlAdapter(mandateRepository.Object);

            var res = await adapter.GetAllCollectionsAsync(new CollectionQueryDto(null, null, null, null, null, null, null, null, Mandate.SortOrder.Ascending, Mandate.CollectionSortCriteria.Name, default));

            Counters expectedCounters = new Counters(1, 0, 0, 0, 0, 0);
            List<Collection> expectedCollections = new List<Collection>()
            {
                coll.ToModel(),
            };

            res.Should().NotBeNull();
            res.Should().BeEquivalentTo(new PagedMandate(expectedCounters, expectedCollections));

            mandateRepository.VerifyAll();
        }

        [Fact]
        public async Task GetCollectionById()
        {
            var collectionId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
            var companyId = Guid.Parse("b1111111-1111-1111-1111-111111111111");

            var collection = new CollectionDb()
            {
                Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                CompanyId = Guid.Parse("b1111111-1111-1111-1111-111111111111"),
                BankCode = "12345",
                BranchCode = "23456",
                AccountNumber = "12345678901",
                CheckDigits = "55",
                LinkType = 7,
                RejectReason = "reason1",
            };

            var refStatusCode = new RefStatusCodeDb()
            {
                StatusCode = -1,
                PulseCode = 30,
                StatusNameFr = "En cours",
                StatusNameEn = "In progress",
            };
            var statusdb = new StatusDb()
            {
                Id = Guid.Parse("c1111111-1111-1111-1111-111111111111"),
                CollectionId = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                StatusCode = -1,
                IsCurrent = true,
                StatusDate = new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc),
                MandateFile = null,
                CreatedBy = "created1",
                RefStatusCode = refStatusCode,
            };

            collection.Company = new CompanyDb()
            {
                Id = companyId,
                Name = "cn1",
                SiretNumber = "12345678901234",
                ErpId = "1234567890",
            };
            collection.Bank = EntityDbFactory.RefBankDb;
            collection.Statuses = new List<StatusDb>() { statusdb };

            var company = new Mandate.Company(companyId, "cn1", "12345678901234", "1234567890", null, null, null);
            var bankAgreement = new BankAgreement(Mandate.JdcPartnership.NonPartner);

            var bank = new Bank("12345", "bn1", "bg", null, bankAgreement);
            var bban = new Mandate.Bban("12345", "23456", "12345678901", "55", null, bank);
            var status = new Status(CollectionStatus.InProgress, "En cours");

            var mandateRepository = new Mock<IMandateRepository>(MockBehavior.Strict);
            mandateRepository.Setup(r => r.GetCollectionById(It.IsAny<Guid>()))
                .Callback<Guid>(id =>
                {
                    id.Should().Be(collectionId);
                })
                .ReturnsAsync(collection)
                .Verifiable();

            var adapter = new SqlAdapter(mandateRepository.Object);
            var result = await adapter.GetCollectionById(collection.Id);

            var expectedCollection = new Collection(
                collection.Id,
                null,
                company,
                bban,
                new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc),
                status);

            result.Should().BeEquivalentTo(expectedCollection);
        }

        [Fact]
        public async Task SaveSignatoryAsync()
        {
            var companyId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
            var collectionId = Guid.Parse("b1111111-1111-1111-1111-111111111111");
            var adress = new Address("1 Rue du Capitaine Floch", "appt 45", "72000", "Le Mans", "FRANCE");
            var signatory = new Signatory("M", "Ludovic", "TYREL DE POIX", "lu.de.poix@mvo-h.fr");

            var repository = new Mock<IMandateRepository>(MockBehavior.Strict);
            repository.Setup(r => r.SaveSignatoryAsync(It.IsAny<PersonalDb>()))
                .Callback<PersonalDb>(p =>
                {
                    p.CompanyId.Should().Be(companyId);
                    p.CollectionId.Should().Be(collectionId);
                    p.City.Should().BeEquivalentTo("Le Mans");
                    p.Country.Should().BeEquivalentTo("FRANCE");
                    p.Street.Should().BeEquivalentTo("1 Rue du Capitaine Floch");
                    p.ZipCode.Should().BeEquivalentTo("72000");
                    p.Complements.Should().BeEquivalentTo("appt 45");
                    p.FirstName.Should().BeEquivalentTo("Ludovic");
                    p.LastName.Should().BeEquivalentTo("TYREL DE POIX");
                    p.Email.Should().BeEquivalentTo("lu.de.poix@mvo-h.fr");
                    p.Title.Should().BeEquivalentTo("M");
                })
                .Returns(Task.CompletedTask)
                .Verifiable();

            var adapter = new SqlAdapter(repository.Object);
            await adapter.SaveSignatoryAsync(companyId, collectionId, signatory, adress);

            repository.VerifyAll();
        }

        [Fact]
        public async Task GetCompanyBySiretAsync()
        {
            var companyId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
            var collectionId = Guid.Parse("b1111111-1111-1111-1111-111111111111");
            var siret = "siretM";

            var companydb = new CompanyDb()
            {
                Id = companyId,
                Name = "Microsoft",
                Personal = new PersonalDb
                {
                    CollectionId = collectionId,
                    CompanyId = companyId,
                    Title = "Mr.",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Street = "123 Main St",
                    Complements = "Apt 4B",
                    ZipCode = "12345",
                    City = "Sample City",
                    Country = "ExampleLand",
                },
                SiretNumber = "40902900600031",
                ErpId = "1000265308",
                JeDeclareFolder = new JeDeclareFolderDb()
                {
                    JdcDossierId = "bankServicesProviderIdT",
                },
            };

            var repository = new Mock<IMandateRepository>(MockBehavior.Strict);
            repository.Setup(r => r.GetCompanyBySiretAsync(siret))
                .ReturnsAsync(companydb)
                .Verifiable();

            var expectedAdress = new Address("123 Main St", "Apt 4B", "12345", "Sample City", "ExampleLand");
            var expectedSignatory = new Signatory("Mr.", "John", "Doe", "john.doe@example.com");
            var expectedResult = new Company(companyId, "Microsoft", "40902900600031", "1000265308", "bankServicesProviderIdT", expectedSignatory, expectedAdress);

            var adapter = new SqlAdapter(repository.Object);

            var result = await adapter.GetCompanyBySiretAsync(siret);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync()
        {
            // Arrange
            var companyId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
            var collectionId = Guid.Parse("b1111111-1111-1111-1111-111111111111");
            var erpId = "erpId";

            var companydb = new CompanyDb()
            {
                Id = companyId,
                Name = "Microsoft",
                Personal = new PersonalDb
                {
                    CollectionId = collectionId,
                    CompanyId = companyId,
                    Title = "Mr.",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Street = "123 Main St",
                    Complements = "Apt 4B",
                    ZipCode = "12345",
                    City = "Sample City",
                    Country = "ExampleLand",
                },
                SiretNumber = "40902900600031",
                ErpId = "1000265308",
                JeDeclareFolder = new JeDeclareFolderDb()
                {
                    JdcDossierId = "bankServicesProviderIdT",
                },
            };

            var repository = new Mock<IMandateRepository>(MockBehavior.Strict);
            repository.Setup(r => r.GetCompanyByErpIdAsync(erpId))
                .ReturnsAsync(companydb)
                .Verifiable();

            var expectedAdress = new Address("123 Main St", "Apt 4B", "12345", "Sample City", "ExampleLand");
            var expectedSignatory = new Signatory("Mr.", "John", "Doe", "john.doe@example.com");
            var expectedResult = new Company(companyId, "Microsoft", "40902900600031", "1000265308", "bankServicesProviderIdT", expectedSignatory, expectedAdress);

            var adapter = new SqlAdapter(repository.Object);

            // Act
            var result = await adapter.GetCompanyByErpIdAsync(erpId);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            repository.Verify(r => r.GetCompanyByErpIdAsync(erpId), Times.Once);
        }
    }
}
