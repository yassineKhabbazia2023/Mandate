// <copyright file="SqlMandateRepositoryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
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
        public async Task Constructor()
        {
            await using var database = SqlServerFixture.CreateDatabase();

            var options = Options.Create(new SqlMandateRepositoryOptions() { ConnectionString = SqlServerFixture.ConnectionString });
            var guidGenerator = new Mock<IGuidGenerator>(MockBehavior.Strict);
            var sqlCalendarRepository = new SqlMandateRepository(options);

            sqlCalendarRepository.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_OptionsNullException()
        {
            IOptions<SqlMandateRepositoryOptions>? options = null;

            var guidGenerator = new Mock<IGuidGenerator>(MockBehavior.Strict);

            Action action = () => { _ = new SqlMandateRepository(options!); };

            action.Should().ThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'options')");
        }

        [Fact]
        public void Constructor_OptionsInvalid()
        {
            var options = Options.Create(new SqlMandateRepositoryOptions() { ConnectionString = null });
            var guidGenerator = new Mock<IGuidGenerator>(MockBehavior.Strict);

            Action creationWithException = () => { _ = new SqlMandateRepository(options); };

            creationWithException.Should().ThrowExactly<InvalidOperationException>().WithMessage("Instance of SqlMandateRepositoryOptions is invalid, ConnectionString is null");
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
            var refStatusCode3 = EntityDbFactory.RefStatusCodeDb;
            refStatusCode2.StatusCode = 3;
            refStatusCode2.CollectionStatusCode = 0;
            refStatusCode2.PulseCode = 30;
            refStatusCode2.StatusNameFr = "Actif";
            refStatusCode3.StatusCode = 4;
            refStatusCode3.CollectionStatusCode = 2;
            refStatusCode3.PulseCode = 40;
            refStatusCode3.StatusNameFr = "Attente mandat signé";
            await context.RefStatusCode.AddAsync(refStatusCode1);
            await context.RefStatusCode.AddAsync(refStatusCode2);
            await context.RefStatusCode.AddAsync(refStatusCode3);

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
            var collection23 = EntityDbFactory.CollectionDb;
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
            collection23.Id = new PredictableGuid(221).NewGuid();
            collection23.CompanyId = new PredictableGuid(202).NewGuid();
            collection23.BankCode = "12346";
            collection23.AccountNumber = "12345678905";
            await context.Collection.AddAsync(collection11);
            await context.Collection.AddAsync(collection12);
            await context.Collection.AddAsync(collection21);
            await context.Collection.AddAsync(collection22);
            await context.Collection.AddAsync(collection23);

            var status111 = EntityDbFactory.StatusDb;
            var status121 = EntityDbFactory.StatusDb;
            var status122 = EntityDbFactory.StatusDb;
            var status211 = EntityDbFactory.StatusDb;
            var status221 = EntityDbFactory.StatusDb;
            var status222 = EntityDbFactory.StatusDb;
            var status223 = EntityDbFactory.StatusDb;
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
            status222.Id = new PredictableGuid(134).NewGuid();
            status222.CollectionId = new PredictableGuid(221).NewGuid();
            status222.StatusDate = new DateTime(2023, 9, 20, 9, 0, 0, DateTimeKind.Utc);
            status222.IsCurrent = false;
            status223.Id = new PredictableGuid(135).NewGuid();
            status223.CollectionId = new PredictableGuid(221).NewGuid();
            status223.StatusDate = new DateTime(2023, 9, 20, 9, 0, 0, DateTimeKind.Utc);
            status223.StatusCode = 4;
            await context.Status.AddAsync(status111);
            await context.Status.AddAsync(status121);
            await context.Status.AddAsync(status122);
            await context.Status.AddAsync(status211);
            await context.Status.AddAsync(status221);
            await context.Status.AddAsync(status222);
            await context.Status.AddAsync(status223);

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
            response11.Item2.Should().Be(5);
            response11.Item1.Count.Should().Be(5);
            response11.Item1[0].AccountNumber.Should().Be("12345678901");
            response11.Item1[1].AccountNumber.Should().Be("12345678902");
            response11.Item1[2].AccountNumber.Should().Be("12345678903");
            response11.Item1[3].AccountNumber.Should().Be("12345678904");
            response11.Item1[4].AccountNumber.Should().Be("12345678905");

            // Sort descending
            var query12 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Descending,
            };
            var response12 = await sqlMandateRepository.SearchCollectionsAsync(query12);
            response12.Item2.Should().Be(5);
            response12.Item1.Count.Should().Be(5);
            response12.Item1[0].AccountNumber.Should().Be("12345678905");
            response12.Item1[1].AccountNumber.Should().Be("12345678904");
            response12.Item1[2].AccountNumber.Should().Be("12345678903");
            response12.Item1[3].AccountNumber.Should().Be("12345678902");
            response12.Item1[4].AccountNumber.Should().Be("12345678901");

            // Sort by bank name
            var query21 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.BankName,
                SortOrder = SortOrder.Descending,
            };
            var response21 = await sqlMandateRepository.SearchCollectionsAsync(query21);
            response21.Item2.Should().Be(5);
            response21.Item1.Count.Should().Be(5);
            response21.Item1[0].Bank!.BankName.Should().Be("bn3");
            response21.Item1[1].Bank!.BankName.Should().Be("bn2");
            response21.Item1[2].Bank!.BankName.Should().Be("bn2");
            response21.Item1[3].Bank!.BankName.Should().Be("bn2");
            response21.Item1[4].Bank!.BankName.Should().Be("bn1");

            // Sort by creation date
            var query22 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.CreationDate,
                SortOrder = SortOrder.Ascending,
            };
            var response22 = await sqlMandateRepository.SearchCollectionsAsync(query22);
            response22.Item2.Should().Be(5);
            response22.Item1.Count.Should().Be(5);
            response22.Item1[0].AccountNumber.Should().Be("12345678902");
            response22.Item1[0].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 19, 9, 0, 0, DateTimeKind.Utc));
            response22.Item1[1].AccountNumber.Should().Be("12345678905");
            response22.Item1[1].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 20, 9, 0, 0, DateTimeKind.Utc));
            response22.Item1[2].AccountNumber.Should().Be("12345678904");
            response22.Item1[2].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 22, 9, 0, 0, DateTimeKind.Utc));
            response22.Item1[3].AccountNumber.Should().Be("12345678901");
            response22.Item1[3].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc));
            response22.Item1[4].AccountNumber.Should().Be("12345678903");
            response22.Item1[4].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 29, 9, 0, 0, DateTimeKind.Utc));

            // Sort by creation date desc OK
            var query29 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.CreationDate,
                SortOrder = SortOrder.Descending,
            };
            var response29 = await sqlMandateRepository.SearchCollectionsAsync(query29);
            response29.Item2.Should().Be(5);
            response29.Item1.Count.Should().Be(5);
            response29.Item1[0].AccountNumber.Should().Be("12345678903");
            response29.Item1[0].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 29, 9, 0, 0, DateTimeKind.Utc));
            response29.Item1[1].AccountNumber.Should().Be("12345678901");
            response29.Item1[1].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc));
            response29.Item1[2].AccountNumber.Should().Be("12345678904");
            response29.Item1[2].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 22, 9, 0, 0, DateTimeKind.Utc));
            response29.Item1[3].AccountNumber.Should().Be("12345678905");
            response29.Item1[3].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 20, 9, 0, 0, DateTimeKind.Utc));
            response29.Item1[4].AccountNumber.Should().Be("12345678902");
            response29.Item1[4].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 19, 9, 0, 0, DateTimeKind.Utc));

            // Sort by modification date
            var query23 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.ModificationDate,
                SortOrder = SortOrder.Ascending,
            };
            var response23 = await sqlMandateRepository.SearchCollectionsAsync(query23);
            response23.Item2.Should().Be(5);
            response23.Item1.Count.Should().Be(5);
            response23.Item1[0].AccountNumber.Should().Be("12345678905"); // 20/09
            response23.Item1[1].AccountNumber.Should().Be("12345678904"); // 22/09
            response23.Item1[2].AccountNumber.Should().Be("12345678901"); // 28/09
            response23.Item1[3].AccountNumber.Should().Be("12345678903"); // 29/09
            response23.Item1[4].AccountNumber.Should().Be("12345678902"); // 30/09

            // Sort by modification date desc ok
            var query28 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.ModificationDate,
                SortOrder = SortOrder.Descending,
            };
            var response28 = await sqlMandateRepository.SearchCollectionsAsync(query28);
            response28.Item2.Should().Be(5);
            response28.Item1.Count.Should().Be(5);
            response28.Item1[0].AccountNumber.Should().Be("12345678902"); // 30/09
            response28.Item1[1].AccountNumber.Should().Be("12345678903"); // 29/09
            response28.Item1[2].AccountNumber.Should().Be("12345678901"); // 28/09
            response28.Item1[3].AccountNumber.Should().Be("12345678904"); // 22/09
            response28.Item1[4].AccountNumber.Should().Be("12345678905"); // 20/09

            // Sort by company name desc
            var query24 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Name,
                SortOrder = SortOrder.Descending,
            };
            var response24 = await sqlMandateRepository.SearchCollectionsAsync(query24);
            response24.Item2.Should().Be(5);
            response24.Item1.Count.Should().Be(5);
            response24.Item1[0].Company!.Name.Should().Be("cn2");
            response24.Item1[1].Company!.Name.Should().Be("cn2");
            response24.Item1[2].Company!.Name.Should().Be("cn2");
            response24.Item1[3].Company!.Name.Should().Be("cn1");
            response24.Item1[4].Company!.Name.Should().Be("cn1");

            // Sort by company name
            var queryc9 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Name,
                SortOrder = SortOrder.Ascending,
            };
            var responsec9 = await sqlMandateRepository.SearchCollectionsAsync(queryc9);
            responsec9.Item2.Should().Be(5);
            responsec9.Item1.Count.Should().Be(5);
            responsec9.Item1[0].Company!.Name.Should().Be("cn1");
            responsec9.Item1[1].Company!.Name.Should().Be("cn1");
            responsec9.Item1[2].Company!.Name.Should().Be("cn2");
            responsec9.Item1[3].Company!.Name.Should().Be("cn2");
            responsec9.Item1[4].Company!.Name.Should().Be("cn2");

            // Sort by erp
            var query25 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.ErpId,
                SortOrder = SortOrder.Ascending,
            };
            var response25 = await sqlMandateRepository.SearchCollectionsAsync(query25);
            response25.Item2.Should().Be(5);
            response25.Item1.Count.Should().Be(5);
            response25.Item1[0].Company!.ErpId.Should().Be("1234567890");
            response25.Item1[1].Company!.ErpId.Should().Be("1234567890");
            response25.Item1[2].Company!.ErpId.Should().Be("9876543210");
            response25.Item1[3].Company!.ErpId.Should().Be("9876543210");
            response25.Item1[4].Company!.ErpId.Should().Be("9876543210");

            // Sort by erp desc
            var queryErp = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.ErpId,
                SortOrder = SortOrder.Descending,
            };
            var responseErp = await sqlMandateRepository.SearchCollectionsAsync(queryErp);
            responseErp.Item2.Should().Be(5);
            responseErp.Item1.Count.Should().Be(5);
            responseErp.Item1[0].Company!.ErpId.Should().Be("9876543210");
            responseErp.Item1[1].Company!.ErpId.Should().Be("9876543210");
            responseErp.Item1[2].Company!.ErpId.Should().Be("9876543210");
            responseErp.Item1[3].Company!.ErpId.Should().Be("1234567890");
            responseErp.Item1[4].Company!.ErpId.Should().Be("1234567890");

            // Sort by status
            var query26 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Status,
                SortOrder = SortOrder.Ascending,
            };
            var response26 = await sqlMandateRepository.SearchCollectionsAsync(query26);
            response26.Item2.Should().Be(5);
            response26.Item1.Count.Should().Be(5);
            response26.Item1[0].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(30);
            response26.Item1[1].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(40);
            response26.Item1[2].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            response26.Item1[3].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            response26.Item1[4].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);

            // Sort by status desc
            var querySt = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Status,
                SortOrder = SortOrder.Descending,
            };
            var responseSt = await sqlMandateRepository.SearchCollectionsAsync(querySt);
            responseSt.Item2.Should().Be(5);
            responseSt.Item1.Count.Should().Be(5);
            responseSt.Item1[0].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            responseSt.Item1[1].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            responseSt.Item1[2].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            responseSt.Item1[3].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(40);
            responseSt.Item1[4].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(30);

            // Page 0
            var query31 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                Skip = 0,
                Limit = 2,
            };
            var response31 = await sqlMandateRepository.SearchCollectionsAsync(query31);
            response31.Item2.Should().Be(5);
            response31.Item1.Count.Should().Be(2);
            response31.Item1[0].AccountNumber.Should().Be("12345678901");
            response31.Item1[1].AccountNumber.Should().Be("12345678902");

            // Page 1
            var query32 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                Skip = 2,
                Limit = 2,
            };
            var response32 = await sqlMandateRepository.SearchCollectionsAsync(query32);
            response32.Item2.Should().Be(5);
            response32.Item1.Count.Should().Be(2);
            response32.Item1[0].AccountNumber.Should().Be("12345678903");
            response32.Item1[1].AccountNumber.Should().Be("12345678904");

            // Partial last page
            var query33 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                Skip = 3,
                Limit = 100,
            };
            var response33 = await sqlMandateRepository.SearchCollectionsAsync(query33);
            response33.Item2.Should().Be(5);
            response33.Item1.Count.Should().Be(2);
            response33.Item1[0].AccountNumber.Should().Be("12345678904");
            response33.Item1[1].AccountNumber.Should().Be("12345678905");

            // After last page
            var query34 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                Skip = 100,
                Limit = 100,
            };
            var response34 = await sqlMandateRepository.SearchCollectionsAsync(query34);
            response34.Item2.Should().Be(5);
            response34.Item1.Count.Should().Be(0);

            // filter + pagination + count
            var query35 = new CollectionQuery()
            {
                SearchTerm = "bn2",
                SortCriteria = CollectionSortCriteria.BankName,
                SortOrder = SortOrder.Ascending,
                Skip = 0,
                Limit = 2,
            };
            var response35 = await sqlMandateRepository.SearchCollectionsAsync(query35);
            response35.Item2.Should().Be(3);
            response35.Item1.Count.Should().Be(2);

            // Filter no result
            var query41 = new CollectionQuery()
            {
                SearchTerm = "boom",
            };
            var response41 = await sqlMandateRepository.SearchCollectionsAsync(query41);
            response41.Item2.Should().Be(0);
            response41.Item1.Count.Should().Be(0);

            // Filter search term on account number OK
            var query56 = new CollectionQuery()
            {
                SearchTerm = "12345678901",
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response56 = await sqlMandateRepository.SearchCollectionsAsync(query56);
            response56.Item2.Should().Be(1);
            response56.Item1.Count.Should().Be(1);
            response56.Item1[0].AccountNumber.Should().Be("12345678901");

            // Filter search term on company name
            var query58 = new CollectionQuery()
            {
                SearchTerm = "cn2",
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response58 = await sqlMandateRepository.SearchCollectionsAsync(query58);
            response58.Item2.Should().Be(3);
            response58.Item1.Count.Should().Be(3);
            response58.Item1[0].AccountNumber.Should().Be("12345678903");
            response58.Item1[1].AccountNumber.Should().Be("12345678904");
            response58.Item1[2].AccountNumber.Should().Be("12345678905");

            // Filter search term on bank name
            var query45 = new CollectionQuery()
            {
                SearchTerm = "bn2",
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response45 = await sqlMandateRepository.SearchCollectionsAsync(query45);
            response45.Item2.Should().Be(3);
            response45.Item1.Count.Should().Be(3);
            response45.Item1[0].AccountNumber.Should().Be("12345678902");
            response45.Item1[1].AccountNumber.Should().Be("12345678904");
            response45.Item1[2].AccountNumber.Should().Be("12345678905");

            // Filter search term on bank name
            var query59 = new CollectionQuery()
            {
                SearchTerm = "1234567890",
                SortCriteria = CollectionSortCriteria.ErpId,
                SortOrder = SortOrder.Ascending,
            };
            var response59 = await sqlMandateRepository.SearchCollectionsAsync(query59);
            response59.Item2.Should().Be(5);
            response59.Item1.Count.Should().Be(5);
            response59.Item1[0].AccountNumber.Should().Be("12345678901");
            response59.Item1[1].AccountNumber.Should().Be("12345678902");
            response59.Item1[2].AccountNumber.Should().Be("12345678903");
            response59.Item1[3].AccountNumber.Should().Be("12345678904");
            response59.Item1[4].AccountNumber.Should().Be("12345678905");

            // Filter creation date
            var query42 = new CollectionQuery()
            {
                CreationDateStart = new DateTime(2023, 9, 12, 9, 0, 0, DateTimeKind.Utc),
                CreationDateEnd = new DateTime(2023, 9, 24, 9, 0, 0, DateTimeKind.Utc),
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response42 = await sqlMandateRepository.SearchCollectionsAsync(query42);
            response42.Item2.Should().Be(3);
            response42.Item1.Count.Should().Be(3);
            response42.Item1[0].AccountNumber.Should().Be("12345678902");
            response42.Item1[1].AccountNumber.Should().Be("12345678904");
            response42.Item1[2].AccountNumber.Should().Be("12345678905");

            // Filter modification date
            var query43 = new CollectionQuery()
            {
                ModificationDateStart = new DateTime(2023, 9, 12, 15, 0, 0, DateTimeKind.Utc),
                ModificationDateEnd = new DateTime(2023, 9, 29, 15, 0, 0, DateTimeKind.Utc),
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response43 = await sqlMandateRepository.SearchCollectionsAsync(query43);
            response43.Item2.Should().Be(4);
            response43.Item1.Count.Should().Be(4);
            response43.Item1[0].AccountNumber.Should().Be("12345678901");
            response43.Item1[1].AccountNumber.Should().Be("12345678903");
            response43.Item1[2].AccountNumber.Should().Be("12345678904");
            response43.Item1[3].AccountNumber.Should().Be("12345678905");

            // Filter on status = 3 & 4 ok
            var query46 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 30, 40 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response46 = await sqlMandateRepository.SearchCollectionsAsync(query46);
            response46.Item2.Should().Be(2);
            response46.Item1.Count.Should().Be(2);
            response46.Item1[0].AccountNumber.Should().Be("12345678902");
            response46.Item1[1].AccountNumber.Should().Be("12345678905");

            // Filter on status = 2
            var query47 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 100 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response47 = await sqlMandateRepository.SearchCollectionsAsync(query47);
            response47.Item2.Should().Be(3);
            response47.Item1.Count.Should().Be(3);
            response47.Item1[0].AccountNumber.Should().Be("12345678901");
            response47.Item1[1].AccountNumber.Should().Be("12345678903");
            response47.Item1[2].AccountNumber.Should().Be("12345678904");

            // Filter on all statuses
            var query48 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 30, 40, 100 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response48 = await sqlMandateRepository.SearchCollectionsAsync(query48);
            response48.Item2.Should().Be(5);
            response48.Item1.Count.Should().Be(5);

            // Filter no result
            var query49 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 3 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
            };
            var response49 = await sqlMandateRepository.SearchCollectionsAsync(query49);
            response49.Item2.Should().Be(0);
            response49.Item1.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetAllCompaniesByCollaboratorAsync()
        {
            // Arrange
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);
            var sqlMandateRepository = new SqlMandateRepository(this.options);

            PredictableGuid generator = new PredictableGuid();
            Guid comapnyId1 = generator.NewGuid();
            Guid comapnyId2 = generator.NewGuid();
            Guid collaboratorId = generator.NewGuid();

            var collectionId1 = generator.NewGuid();
            var collectionId2 = generator.NewGuid();

            var refBankDb = new RefBankDb()
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

            await context.RefBank.AddAsync(refBankDb);

            var collectiondb1 = new CollectionDb()
            {
                Id = collectionId1,
                CompanyId = comapnyId1,
                BankCode = "12345",
                BranchCode = "23456",
                AccountNumber = "12345678901",
                CheckDigits = "55",
                LinkType = 7,
                RejectReason = "reason1",
            };

            var collectiondb2 = new CollectionDb()
            {
                Id = collectionId2,
                CompanyId = comapnyId2,
                BankCode = "12345",
                BranchCode = "23456",
                AccountNumber = "12345678901",
                CheckDigits = "55",
                LinkType = 7,
                RejectReason = "reason1",
            };

            await context.Collection.AddAsync(collectiondb1);
            await context.Collection.AddAsync(collectiondb2);

            CompanyDb company1 = new CompanyDb
            {
                Id = comapnyId1,
                Personal = new PersonalDb
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
                },
                Name = "Microsoft",
                SiretNumber = "40902900600031",
                ErpId = "1000265308",
            };
            await context.Company.AddAsync(company1);

            CompanyDb company2 = new CompanyDb
            {
                Id = comapnyId2,
                Personal = new PersonalDb
                {
                    CollectionId = collectionId2,
                    CompanyId = comapnyId2,
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
                Name = "Dior",
                SiretNumber = "40930900600031",
                ErpId = "1000265309",
            };
            await context.Company.AddAsync(company2);

            CollaboratorDb collaborator = new CollaboratorDb
            {
                Id = collaboratorId,
                Email = "smedini@kpmg.fr",
                FirstName = "Seif Allah",
                LastName = "MEDINI",
            };
            await context.Collaborator.AddAsync(collaborator);

            CompanyCollaboratorDb companyCollaborator1 = new CompanyCollaboratorDb
            {
                CompanyId = comapnyId1,
                Company = company1,
                CollaboratorId = collaboratorId,
                Collaborator = collaborator,
            };
            await context.CompanyCollaborator.AddAsync(companyCollaborator1);

            CompanyCollaboratorDb companyCollaborator2 = new CompanyCollaboratorDb
            {
                CompanyId = comapnyId2,
                Company = company2,
                CollaboratorId = collaboratorId,
                Collaborator = collaborator,
            };
            await context.CompanyCollaborator.AddAsync(companyCollaborator2);

            await context.SaveChangesAsync();

            // Act
            List<CompanyDb?> companies = await sqlMandateRepository.GetAllCompaniesByCollaboratorAsync("smedini@kpmg.fr");

            // Assert
            companies.Count.Should().Be(2);
            companies[0]?.Name.Should().Be("Microsoft");
            companies[0]?.SiretNumber.Should().Be("40902900600031");
            companies[0]?.ErpId.Should().Be("1000265308");
            companies[1]?.Name.Should().Be("Dior");
            companies[1]?.SiretNumber.Should().Be("40930900600031");
            companies[1]?.ErpId.Should().Be("1000265309");
        }

        [Fact]
        public async Task GetCollectionById()
        {
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            var collectionId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
            var companyId = Guid.Parse("b1111111-1111-1111-1111-111111111111");
            var companyId2 = Guid.Parse("c1111111-1111-1111-1111-111111111111");

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

            var collection2 = new CollectionDb()
            {
                Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                CompanyId = Guid.Parse("c1111111-1111-1111-1111-111111111111"),
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

            var companydb = new CompanyDb()
            {
                Id = companyId,
                Name = "cn1",
                SiretNumber = "12345678901234",
                ErpId = "1234567890",
            };

            var companydb2 = new CompanyDb()
            {
                Id = companyId2,
                Name = "cn1",
                SiretNumber = "12345678901234",
                ErpId = "1234567890",
            };
            var bankDb = EntityDbFactory.RefBankDb;

            await context.Collection.AddAsync(collection);
            await context.Collection.AddAsync(collection2);
            await context.Company.AddAsync(companydb);
            await context.Company.AddAsync(companydb2);
            await context.RefBank.AddAsync(bankDb);
            await context.RefStatusCode.AddAsync(refStatusCode);
            await context.Status.AddAsync(statusdb);

            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(this.options);

            var result = await sqlMandateRepository.GetCollectionById(collectionId);

            result.Should().NotBeNull();
            result.Id.Should().Be(Guid.Parse("a1111111-1111-1111-1111-111111111111"));
        }

        [Fact]
        public async Task SaveSignatoryAsync_CaseNewEntity()
        {
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            var companyId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
            var collectionId = Guid.Parse("b1111111-1111-1111-1111-111111111111");

            var refBankDb = new RefBankDb()
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

            await context.RefBank.AddAsync(refBankDb);

            var collectiondb1 = new CollectionDb()
            {
                Id = collectionId,
                CompanyId = companyId,
                BankCode = "12345",
                BranchCode = "23456",
                AccountNumber = "12345678901",
                CheckDigits = "55",
                LinkType = 7,
                RejectReason = "reason1",
            };

            await context.Collection.AddAsync(collectiondb1);

            CompanyDb company1 = new CompanyDb
            {
                Id = companyId,
                Name = "Microsoft",
                SiretNumber = "40902900600031",
                ErpId = "1000265308",
            };
            await context.Company.AddAsync(company1);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(this.options);

            var personal = new PersonalDb
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
            };
            await sqlMandateRepository.SaveSignatoryAsync(personal);
            var db = database.ExecuteQuery("select * from [Mandate].[Personal]");

            db.Rows.Count.Should().Be(1);
            var dbr0 = db.Rows[0];
            dbr0["CollectionId"].As<Guid>().Should().Be(collectionId);
            dbr0["Title"].As<string>().Should().BeEquivalentTo("Mr.");
            dbr0["FirstName"].As<string>().Should().BeEquivalentTo("John");
            dbr0["LastName"].As<string>().Should().BeEquivalentTo("Doe");
            dbr0["Email"].As<string>().Should().BeEquivalentTo("john.doe@example.com");
            dbr0["Street"].As<string>().Should().BeEquivalentTo("123 Main St");
            dbr0["Complements"].As<string>().Should().BeEquivalentTo("Apt 4B");
            dbr0["ZipCode"].As<string>().Should().BeEquivalentTo("12345");
            dbr0["City"].As<string>().Should().BeEquivalentTo("Sample City");
            dbr0["Country"].As<string>().Should().BeEquivalentTo("ExampleLand");
        }

        [Fact]
        public async Task FakeData()
        {
            await using var database = SqlServerFixture.CreateDatabase();
            var sqlMandateRepository = new SqlMandateRepository(this.options);
            await sqlMandateRepository.CreateFakeRefAsync();
            await sqlMandateRepository.AddFakeDataAsync();
            await sqlMandateRepository.CreateFakeAuthAsync();

            using var context = new MandateContext(this.options);
            context.RefPdfTemplate.Count().Should().Be(1);
            context.RefBank.Count().Should().Be(10);
            context.RefBank.Count(b => b.JdcPartnership == JdcPartnership.NonPartner).Should().Be(1);
            context.RefBank.Count(b => b.JdcPartnership == JdcPartnership.Scrappable).Should().Be(1);
            context.RefStatusCode.Count().Should().Be(19);
            context.Collaborator.Count().Should().Be(9);
            context.CompanyCollaborator.Count().Should().Be(9);
            context.Company.Count().Should().Be(1);
            context.Personal.Count().Should().Be(3);
            context.JeDeclareFolder.Count().Should().Be(1);
            context.Collection.Count().Should().Be(2);
            context.JeDeclareCollection.Count().Should().Be(2);
            context.Status.Count().Should().Be(4);

            await sqlMandateRepository.DeleteFakeAuthAsync();
            context.Collaborator.Count().Should().Be(0);
            context.CompanyCollaborator.Count().Should().Be(0);
            context.Company.Count().Should().Be(1);

            await sqlMandateRepository.CreateFakeAuthAsync();
            context.Collaborator.Count().Should().Be(9);
            context.CompanyCollaborator.Count().Should().Be(9);

            await sqlMandateRepository.DeleteFakeDataAsync();
            context.Collaborator.Count().Should().Be(9);
            context.CompanyCollaborator.Count().Should().Be(0);
            context.Company.Count().Should().Be(0);
            context.Personal.Count().Should().Be(0);
            context.JeDeclareFolder.Count().Should().Be(0);
            context.Collection.Count().Should().Be(0);
            context.JeDeclareCollection.Count().Should().Be(0);
            context.Status.Count().Should().Be(0);

            await sqlMandateRepository.DeleteFakeRefAsync();
            context.RefPdfTemplate.Count().Should().Be(0);
            context.RefBank.Count().Should().Be(0);
            context.RefStatusCode.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_ShouldReturnCompany_WhenCompanyExists()
        {
            // Arrange
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            PredictableGuid generator = new PredictableGuid();
            Guid companyId = generator.NewGuid();  // Ensure this is the same ID used for the foreign key in CollectionDb
            var erpId = "validErpId";

            var refBankDb = new RefBankDb()
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

            await context.RefBank.AddAsync(refBankDb);

            var expectedCompany = new CompanyDb
            {
                Id = companyId,
                Name = "Dior",
                SiretNumber = "40930900600031",
                ErpId = erpId,
            };

            await context.Company.AddAsync(expectedCompany);
            await context.SaveChangesAsync();

            Guid collectionId = generator.NewGuid();
            var collectionDb = new CollectionDb()
            {
                Id = collectionId,
                CompanyId = companyId,  // This must match the ID of the Company record
                BankCode = "12345",
                BranchCode = "23456",
                AccountNumber = "12345678901",
                CheckDigits = "55",
                LinkType = 7,
                RejectReason = "reason1",
            };

            await context.Collection.AddAsync(collectionDb);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(this.options);

            // Act
            var result = await sqlMandateRepository.GetCompanyByErpIdAsync(erpId);

            // Assert
            result.Id.Should().Be(expectedCompany.Id);
            result.ErpId.Should().Be(expectedCompany.ErpId);
            result.SiretNumber.Should().Be(expectedCompany.SiretNumber);
            result.Name.Should().Be(expectedCompany.Name);
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_ShouldThrowCompanyNotFoundException_WhenCompanyDoesNotExist()
        {
            // Arrange
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            var sqlMandateRepository = new SqlMandateRepository(this.options);
            var nonExistingErpId = "nonExistingErpId";

            // Act & Assert
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyByErpIdAsync(nonExistingErpId);

            await act.Should().ThrowAsync<CompanyNotFoundException>();
        }

        [Fact]
        public async Task CreateOrUpdateFolderAsync_Case_Create()
        {
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            var company1 = EntityDbFactory.CompanyDb;
            await context.Company.AddAsync(company1);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(this.options);

            await sqlMandateRepository.CreateOrUpdateFolderAsync("folderId2", new PredictableGuid(102).NewGuid());

            var companyFolder = await context.JeDeclareFolder
                .Where(item => item.CompanyId == new PredictableGuid(102).NewGuid())
                .ToListAsync();

            companyFolder.Count.Should().Be(1);
            companyFolder.Single().CompanyId.Should().Be(new PredictableGuid(102).NewGuid());
            companyFolder.Single().JdcDossierId.Should().Be("folderId2");
        }

        [Fact]
        public async Task CreateOrUpdateFolderAsync_Case_Update()
        {
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            var company1 = EntityDbFactory.CompanyDb;
            await context.Company.AddAsync(company1);
            await context.SaveChangesAsync();

            var jdcFolder = EntityDbFactory.JeDeclareFolderDb;
            await context.JeDeclareFolder.AddAsync(jdcFolder);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(this.options);

            await sqlMandateRepository.CreateOrUpdateFolderAsync("folderId", new PredictableGuid(102).NewGuid());
            var all = await context.JeDeclareFolder.ToListAsync();
            var companyFolder = await context.JeDeclareFolder
                .Where(item => item.CompanyId == new PredictableGuid(102).NewGuid())
                .ToListAsync();

            companyFolder.Count.Should().Be(1);
            companyFolder.Single().CompanyId.Should().Be(new PredictableGuid(102).NewGuid());
            companyFolder.Single().JdcDossierId.Should().Be("folderId");
        }
    }
}
