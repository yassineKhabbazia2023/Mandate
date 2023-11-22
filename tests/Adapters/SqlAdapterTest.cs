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
            mandateRepository.Setup(r => r.SearchCollectionsAsync(It.IsAny<CollectionQuery>()))
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
                BankServicesProviderId = "bankServicesProviderIdM",
            };

            var repository = new Mock<IMandateRepository>(MockBehavior.Strict);
            repository.Setup(r => r.GetCompanyBySiretAsync(siret))
                .ReturnsAsync(companydb)
                .Verifiable();

            var expectedAdress = new Address("123 Main St", "Apt 4B", "12345", "Sample City", "ExampleLand");
            var expectedSignatory = new Signatory("Mr.", "John", "Doe", "john.doe@example.com");
            var expectedResult = new Company(companyId, "Microsoft", "40902900600031", "1000265308", "bankServicesProviderIdM", expectedSignatory, expectedAdress);

            var adapter = new SqlAdapter(repository.Object);

            var result = await adapter.GetCompanyBySiretAsync(siret);
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
