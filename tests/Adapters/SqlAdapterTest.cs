// <copyright file="SqlAdapterTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
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

            var mandateRepository = new Mock<IMandateRepository>(MockBehavior.Strict);
            mandateRepository.Setup(r => r.SearchCollectionsAsync(It.IsAny<Sql.CollectionQuery>()))
                .ReturnsAsync(new List<CollectionDb>() { coll })
                .Verifiable();

            SqlAdapter adapter = new SqlAdapter(mandateRepository.Object);

            var res = await adapter.GetAllCollectionsAsync(new CollectionQueryDto(null, null, null, null, null, null, null, null, Mandate.SortOrder.Ascending, Mandate.CollectionSortCriteria.Name, default));

            res.Should().NotBeNull();
            res.Count().Should().Be(1);
            res.Single().Should().BeEquivalentTo(coll.ToModel());
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
                BankServicesProviderId = null,
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
    }
}
