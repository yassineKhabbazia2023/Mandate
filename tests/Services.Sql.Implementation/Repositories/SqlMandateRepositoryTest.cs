// <copyright file="SqlMandateRepositoryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using Azure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    [Collection("SerialExecutionPublishDb")]
    public class SqlMandateRepositoryTest
    {
        private readonly IOptions<SqlMandateRepositoryOptions> options;

        public SqlMandateRepositoryTest()
        {
            this.options = Options.Create(new SqlMandateRepositoryOptions()
            {
                ConnectionString = SqlServerFixture.ConnectionString,
            });
        }

        [Fact]
        public async Task SearchCollectionsAsync()
        {
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            // Fill database with referencial data
            var refBank1 = EntityDbFactory.RefBankDb;
            var refBank2 = EntityDbFactory.RefBankDb;
            var refBank3 = EntityDbFactory.RefBankDb;
            refBank2.BankCode = "12346";
            refBank2.BankName = "bn2";
            refBank3.BankCode = "12347";
            refBank3.BankName = "bn3";
            await context.RefBank.AddAsync(refBank1);
            await context.RefBank.AddAsync(refBank2);
            await context.RefBank.AddAsync(refBank3);

            var refStatusCode1 = EntityDbFactory.RefStatusCodeDb;
            var refStatusCode2 = EntityDbFactory.RefStatusCodeDb;
            refStatusCode2.StatusCode = 3;
            refStatusCode2.PulseCode = 30;
            refStatusCode2.StatusNameFr = "Actif";
            await context.RefStatusCode.AddAsync(refStatusCode1);
            await context.RefStatusCode.AddAsync(refStatusCode2);

            var company1 = EntityDbFactory.CompanyDb;
            var company2 = EntityDbFactory.CompanyDb;
            company2.Id = new PredictableGuid(202).NewGuid();
            company2.Name = "cn2";
            company2.ErpId = "9876543210";
            await context.Company.AddAsync(company1);
            await context.Company.AddAsync(company2);

            var collection11 = EntityDbFactory.CollectionDb;
            var collection12 = EntityDbFactory.CollectionDb;
            var collection21 = EntityDbFactory.CollectionDb;
            var collection22 = EntityDbFactory.CollectionDb;
            collection12.Id = new PredictableGuid(111).NewGuid();
            collection12.CompanyId = new PredictableGuid(102).NewGuid();
            collection12.BankCode = "12346";
            collection12.AccountNumber = "12345678902";
            collection21.Id = new PredictableGuid(201).NewGuid();
            collection21.CompanyId = new PredictableGuid(202).NewGuid();
            collection21.BankCode = "12347";
            collection21.AccountNumber = "12345678903";
            collection22.Id = new PredictableGuid(211).NewGuid();
            collection22.CompanyId = new PredictableGuid(202).NewGuid();
            collection22.BankCode = "12346";
            collection22.AccountNumber = "12345678904";
            await context.Collection.AddAsync(collection11);
            await context.Collection.AddAsync(collection12);
            await context.Collection.AddAsync(collection21);
            await context.Collection.AddAsync(collection22);

            var status111 = EntityDbFactory.StatusDb;
            var status121 = EntityDbFactory.StatusDb;
            var status122 = EntityDbFactory.StatusDb;
            var status211 = EntityDbFactory.StatusDb;
            var status221 = EntityDbFactory.StatusDb;
            status121.Id = new PredictableGuid(130).NewGuid();
            status121.CollectionId = new PredictableGuid(111).NewGuid();
            status121.StatusDate = new DateTime(2023, 9, 19, 9, 0, 0, DateTimeKind.Utc);
            status121.IsCurrent = false;
            status122.Id = new PredictableGuid(131).NewGuid();
            status122.CollectionId = new PredictableGuid(111).NewGuid();
            status122.StatusCode = 3;
            status122.StatusDate = new DateTime(2023, 9, 30, 9, 0, 0, DateTimeKind.Utc);
            status211.Id = new PredictableGuid(132).NewGuid();
            status211.CollectionId = new PredictableGuid(201).NewGuid();
            status211.StatusDate = new DateTime(2023, 9, 29, 9, 0, 0, DateTimeKind.Utc);
            status221.Id = new PredictableGuid(133).NewGuid();
            status221.CollectionId = new PredictableGuid(211).NewGuid();
            status221.StatusDate = new DateTime(2023, 9, 22, 9, 0, 0, DateTimeKind.Utc);
            await context.Status.AddAsync(status111);
            await context.Status.AddAsync(status121);
            await context.Status.AddAsync(status122);
            await context.Status.AddAsync(status211);
            await context.Status.AddAsync(status221);

            await context.SaveChangesAsync();

            // Instanciate
            var sqlMandateRepository = new SqlMandateRepository(this.options);

            // Tests
            // Sort ascending
            var query11 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response11 = await sqlMandateRepository.SearchCollectionsAsync(query11);
            response11.Count.Should().Be(4);
            response11[0].AccountNumber.Should().Be("12345678901");
            response11[1].AccountNumber.Should().Be("12345678902");
            response11[2].AccountNumber.Should().Be("12345678903");
            response11[3].AccountNumber.Should().Be("12345678904");

            // Sort descending
            var query12 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Descending,
            };
            var response12 = await sqlMandateRepository.SearchCollectionsAsync(query12);
            response12.Count.Should().Be(4);
            response12[0].AccountNumber.Should().Be("12345678904");
            response12[1].AccountNumber.Should().Be("12345678903");
            response12[2].AccountNumber.Should().Be("12345678902");
            response12[3].AccountNumber.Should().Be("12345678901");

            // Sort by bank name
            var query21 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.BankName,
                SortOrder = SortOrder.Descending,
            };
            var response21 = await sqlMandateRepository.SearchCollectionsAsync(query21);
            response21.Count.Should().Be(4);
            response21[0].Bank!.BankName.Should().Be("bn3");
            response21[1].Bank!.BankName.Should().Be("bn2");
            response21[2].Bank!.BankName.Should().Be("bn2");
            response21[3].Bank!.BankName.Should().Be("bn1");

            // Sort by creation date
            var query22 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.CreationDate,
                SortOrder = SortOrder.Ascending,
            };
            var response22 = await sqlMandateRepository.SearchCollectionsAsync(query22);
            response22.Count.Should().Be(4);
            response22[0].AccountNumber.Should().Be("12345678902");
            response22[0].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 19, 9, 0, 0, DateTimeKind.Utc));
            response22[1].AccountNumber.Should().Be("12345678904");
            response22[1].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 22, 9, 0, 0, DateTimeKind.Utc));
            response22[2].AccountNumber.Should().Be("12345678901");
            response22[2].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc));
            response22[3].AccountNumber.Should().Be("12345678903");
            response22[3].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 29, 9, 0, 0, DateTimeKind.Utc));

            // Sort by modification date
            var query23 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.ModificationDate,
                SortOrder = SortOrder.Ascending,
            };
            var response23 = await sqlMandateRepository.SearchCollectionsAsync(query23);
            response23.Count.Should().Be(4);
            response23[0].AccountNumber.Should().Be("12345678904"); // 22/09
            response23[1].AccountNumber.Should().Be("12345678901"); // 28/09
            response23[2].AccountNumber.Should().Be("12345678903"); // 29/09
            response23[3].AccountNumber.Should().Be("12345678902"); // 30/09

            // Sort by company name
            var query24 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Name,
                SortOrder = SortOrder.Descending,
            };
            var response24 = await sqlMandateRepository.SearchCollectionsAsync(query24);
            response24.Count.Should().Be(4);
            response24[0].Company!.Name.Should().Be("cn2");
            response24[1].Company!.Name.Should().Be("cn2");
            response24[2].Company!.Name.Should().Be("cn1");
            response24[3].Company!.Name.Should().Be("cn1");

            // Sort by erp
            var query25 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.ErpId,
                SortOrder = SortOrder.Ascending,
            };
            var response25 = await sqlMandateRepository.SearchCollectionsAsync(query25);
            response25.Count.Should().Be(4);
            response25[0].Company!.ErpId.Should().Be("1234567890");
            response25[1].Company!.ErpId.Should().Be("1234567890");
            response25[2].Company!.ErpId.Should().Be("9876543210");
            response25[3].Company!.ErpId.Should().Be("9876543210");

            // Sort by status
            var query26 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Status,
                SortOrder = SortOrder.Ascending,
            };
            var response26 = await sqlMandateRepository.SearchCollectionsAsync(query26);
            response26.Count.Should().Be(4);
            response26[0].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(30);
            response26[1].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            response26[2].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            response26[3].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);

            // Page 0
            var query31 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                Skip = 0,
                Limit = 2,
            };
            var response31 = await sqlMandateRepository.SearchCollectionsAsync(query31);
            response31.Count.Should().Be(2);
            response31[0].AccountNumber.Should().Be("12345678901");
            response31[1].AccountNumber.Should().Be("12345678902");

            // Page 1
            var query32 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                Skip = 2,
                Limit = 2,
            };
            var response32 = await sqlMandateRepository.SearchCollectionsAsync(query32);
            response32.Count.Should().Be(2);
            response32[0].AccountNumber.Should().Be("12345678903");
            response32[1].AccountNumber.Should().Be("12345678904");

            // Partial last page
            var query33 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                Skip = 3,
                Limit = 100,
            };
            var response33 = await sqlMandateRepository.SearchCollectionsAsync(query33);
            response33.Count.Should().Be(1);
            response33[0].AccountNumber.Should().Be("12345678904");

            // After last page
            var query34 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                Skip = 100,
                Limit = 100,
            };
            var response34 = await sqlMandateRepository.SearchCollectionsAsync(query34);
            response34.Count.Should().Be(0);

            // Filter no result
            var query41 = new CollectionQuery()
            {
                SearchTerm = "boom",
            };
            var response41 = await sqlMandateRepository.SearchCollectionsAsync(query41);
            response41.Count.Should().Be(0);

            // Filter creation date
            var query42 = new CollectionQuery()
            {
                CreationDateStart = new DateTime(2023, 9, 12, 9, 0, 0, DateTimeKind.Utc),
                CreationDateEnd = new DateTime(2023, 9, 24, 9, 0, 0, DateTimeKind.Utc),
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response42 = await sqlMandateRepository.SearchCollectionsAsync(query42);
            response42.Count.Should().Be(2);
            response42[0].AccountNumber.Should().Be("12345678902");
            response42[1].AccountNumber.Should().Be("12345678904");

            // Filter modification date
            var query43 = new CollectionQuery()
            {
                ModificationDateStart = new DateTime(2023, 9, 12, 15, 0, 0, DateTimeKind.Utc),
                ModificationDateEnd = new DateTime(2023, 9, 29, 15, 0, 0, DateTimeKind.Utc),
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response43 = await sqlMandateRepository.SearchCollectionsAsync(query43);
            response43.Count.Should().Be(3);
            response43[0].AccountNumber.Should().Be("12345678901");
            response43[1].AccountNumber.Should().Be("12345678903"); // Excluding 12345678902 because it has a modification date on 30/09
            response43[2].AccountNumber.Should().Be("12345678904");

            // Filter search term on company name
            var query44 = new CollectionQuery()
            {
                SearchTerm = "cn2",
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response44 = await sqlMandateRepository.SearchCollectionsAsync(query44);
            response44.Count.Should().Be(2);
            response44[0].AccountNumber.Should().Be("12345678903");
            response44[1].AccountNumber.Should().Be("12345678904");

            // Filter search term on bank name
            var query45 = new CollectionQuery()
            {
                SearchTerm = "bn2",
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response45 = await sqlMandateRepository.SearchCollectionsAsync(query45);
            response45.Count.Should().Be(2);
            response45[0].AccountNumber.Should().Be("12345678902");
            response45[1].AccountNumber.Should().Be("12345678904");

            // Filter on status = 3
            var query46 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 30 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response46 = await sqlMandateRepository.SearchCollectionsAsync(query46);
            response46.Count.Should().Be(1);
            response46[0].AccountNumber.Should().Be("12345678902");

            // Filter on status = 2
            var query47 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 100 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response47 = await sqlMandateRepository.SearchCollectionsAsync(query47);
            response47.Count.Should().Be(3);
            response47[0].AccountNumber.Should().Be("12345678901");
            response47[1].AccountNumber.Should().Be("12345678903");
            response47[2].AccountNumber.Should().Be("12345678904");

            // Filter on all statuses
            var query48 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 30, 100 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response48 = await sqlMandateRepository.SearchCollectionsAsync(query48);
            response48.Count.Should().Be(4);

            // Filter no result
            var query49 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 3 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response49 = await sqlMandateRepository.SearchCollectionsAsync(query49);
            response49.Count.Should().Be(0);
        }
    }
}
