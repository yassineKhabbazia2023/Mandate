// <copyright file="SqlMandateRepositoryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
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
            var refStatusCode3 = EntityDbFactory.RefStatusCodeDb;
            refStatusCode2.StatusCode = 3;
            refStatusCode2.PulseCode = 30;
            refStatusCode2.StatusNameFr = "Actif";
            refStatusCode3.StatusCode = 4;
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

            var collaborator11 = EntityDbFactory.CollaboratorDb;
            await context.Collaborator.AddAsync(collaborator11);

            var companyCollaborator111 = EntityDbFactory.CompanyCollaboratorDb;
            var companyCollaborator112 = EntityDbFactory.CompanyCollaboratorDb;
            companyCollaborator112.CompanyId = new PredictableGuid(202).NewGuid();
            await context.CompanyCollaborator.AddAsync(companyCollaborator111);
            await context.CompanyCollaborator.AddAsync(companyCollaborator112);

            await context.SaveChangesAsync();

            // Instanciate
            var sqlMandateRepository = new SqlMandateRepository(this.options);

            // Tests
            // Sort ascending
            var query11 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response11 = await sqlMandateRepository.SearchCollectionsAsync(query11);
            response11.Count.Should().Be(5);
            response11[0].AccountNumber.Should().Be("12345678901");
            response11[1].AccountNumber.Should().Be("12345678902");
            response11[2].AccountNumber.Should().Be("12345678903");
            response11[3].AccountNumber.Should().Be("12345678904");
            response11[4].AccountNumber.Should().Be("12345678905");

            // Sort descending
            var query12 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Descending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response12 = await sqlMandateRepository.SearchCollectionsAsync(query12);
            response12.Count.Should().Be(5);
            response12[0].AccountNumber.Should().Be("12345678905");
            response12[1].AccountNumber.Should().Be("12345678904");
            response12[2].AccountNumber.Should().Be("12345678903");
            response12[3].AccountNumber.Should().Be("12345678902");
            response12[4].AccountNumber.Should().Be("12345678901");

            // Sort by bank name
            var query21 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.BankName,
                SortOrder = SortOrder.Descending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response21 = await sqlMandateRepository.SearchCollectionsAsync(query21);
            response21.Count.Should().Be(5);
            response21[0].Bank!.BankName.Should().Be("bn3");
            response21[1].Bank!.BankName.Should().Be("bn2");
            response21[2].Bank!.BankName.Should().Be("bn2");
            response21[3].Bank!.BankName.Should().Be("bn2");
            response21[4].Bank!.BankName.Should().Be("bn1");

            // Sort by creation date
            var query22 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.CreationDate,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response22 = await sqlMandateRepository.SearchCollectionsAsync(query22);
            response22.Count.Should().Be(5);
            response22[0].AccountNumber.Should().Be("12345678902");
            response22[0].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 19, 9, 0, 0, DateTimeKind.Utc));
            response22[1].AccountNumber.Should().Be("12345678905");
            response22[1].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 20, 9, 0, 0, DateTimeKind.Utc));
            response22[2].AccountNumber.Should().Be("12345678904");
            response22[2].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 22, 9, 0, 0, DateTimeKind.Utc));
            response22[3].AccountNumber.Should().Be("12345678901");
            response22[3].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc));
            response22[4].AccountNumber.Should().Be("12345678903");
            response22[4].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 29, 9, 0, 0, DateTimeKind.Utc));

            // Sort by creation date desc OK
            var query29 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.CreationDate,
                SortOrder = SortOrder.Descending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response29 = await sqlMandateRepository.SearchCollectionsAsync(query29);
            response29.Count.Should().Be(5);
            response29[0].AccountNumber.Should().Be("12345678903");
            response29[0].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 29, 9, 0, 0, DateTimeKind.Utc));
            response29[1].AccountNumber.Should().Be("12345678901");
            response29[1].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc));
            response29[2].AccountNumber.Should().Be("12345678904");
            response29[2].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 22, 9, 0, 0, DateTimeKind.Utc));
            response29[3].AccountNumber.Should().Be("12345678905");
            response29[3].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 20, 9, 0, 0, DateTimeKind.Utc));
            response29[4].AccountNumber.Should().Be("12345678902");
            response29[4].Statuses[0].StatusDate.Should().Be(new DateTime(2023, 9, 19, 9, 0, 0, DateTimeKind.Utc));

            // Sort by modification date
            var query23 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.ModificationDate,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response23 = await sqlMandateRepository.SearchCollectionsAsync(query23);
            response23.Count.Should().Be(5);
            response23[0].AccountNumber.Should().Be("12345678905"); // 20/09
            response23[1].AccountNumber.Should().Be("12345678904"); // 22/09
            response23[2].AccountNumber.Should().Be("12345678901"); // 28/09
            response23[3].AccountNumber.Should().Be("12345678903"); // 29/09
            response23[4].AccountNumber.Should().Be("12345678902"); // 30/09

            // Sort by modification date desc ok
            var query28 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.ModificationDate,
                SortOrder = SortOrder.Descending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response28 = await sqlMandateRepository.SearchCollectionsAsync(query28);
            response28.Count.Should().Be(5);
            response28[0].AccountNumber.Should().Be("12345678902"); // 30/09
            response28[1].AccountNumber.Should().Be("12345678903"); // 29/09
            response28[2].AccountNumber.Should().Be("12345678901"); // 28/09
            response28[3].AccountNumber.Should().Be("12345678904"); // 22/09
            response28[4].AccountNumber.Should().Be("12345678905"); // 20/09

            // Sort by company name desc
            var query24 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Name,
                SortOrder = SortOrder.Descending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response24 = await sqlMandateRepository.SearchCollectionsAsync(query24);
            response24.Count.Should().Be(5);
            response24[0].Company!.Name.Should().Be("cn2");
            response24[1].Company!.Name.Should().Be("cn2");
            response24[2].Company!.Name.Should().Be("cn2");
            response24[3].Company!.Name.Should().Be("cn1");
            response24[4].Company!.Name.Should().Be("cn1");

            // Sort by company name
            var queryc9 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Name,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var responsec9 = await sqlMandateRepository.SearchCollectionsAsync(queryc9);
            responsec9.Count.Should().Be(5);
            responsec9[0].Company!.Name.Should().Be("cn1");
            responsec9[1].Company!.Name.Should().Be("cn1");
            responsec9[2].Company!.Name.Should().Be("cn2");
            responsec9[3].Company!.Name.Should().Be("cn2");
            responsec9[4].Company!.Name.Should().Be("cn2");

            // Sort by erp
            var query25 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.ErpId,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response25 = await sqlMandateRepository.SearchCollectionsAsync(query25);
            response25.Count.Should().Be(5);
            response25[0].Company!.ErpId.Should().Be("1234567890");
            response25[1].Company!.ErpId.Should().Be("1234567890");
            response25[2].Company!.ErpId.Should().Be("9876543210");
            response25[3].Company!.ErpId.Should().Be("9876543210");
            response25[4].Company!.ErpId.Should().Be("9876543210");

            // Sort by erp desc
            var queryErp = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.ErpId,
                SortOrder = SortOrder.Descending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var responseErp = await sqlMandateRepository.SearchCollectionsAsync(queryErp);
            responseErp.Count.Should().Be(5);
            responseErp[0].Company!.ErpId.Should().Be("9876543210");
            responseErp[1].Company!.ErpId.Should().Be("9876543210");
            responseErp[2].Company!.ErpId.Should().Be("9876543210");
            responseErp[3].Company!.ErpId.Should().Be("1234567890");
            responseErp[4].Company!.ErpId.Should().Be("1234567890");

            // Sort by status
            var query26 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Status,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response26 = await sqlMandateRepository.SearchCollectionsAsync(query26);
            response26.Count.Should().Be(5);
            response26[0].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(30);
            response26[1].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(40);
            response26[2].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            response26[3].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            response26[4].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);

            // Sort by status desc
            var querySt = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Status,
                SortOrder = SortOrder.Descending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var responseSt = await sqlMandateRepository.SearchCollectionsAsync(querySt);
            responseSt.Count.Should().Be(5);
            responseSt[0].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            responseSt[1].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            responseSt[2].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(100);
            responseSt[3].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(40);
            responseSt[4].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(30);

            // Page 0
            var query31 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
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
                CollaboratorId = new PredictableGuid(104).NewGuid(),
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
                CollaboratorId = new PredictableGuid(104).NewGuid(),
                Skip = 3,
                Limit = 100,
            };
            var response33 = await sqlMandateRepository.SearchCollectionsAsync(query33);
            response33.Count.Should().Be(2);
            response33[0].AccountNumber.Should().Be("12345678904");
            response33[1].AccountNumber.Should().Be("12345678905");

            // After last page
            var query34 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
                Skip = 100,
                Limit = 100,
            };
            var response34 = await sqlMandateRepository.SearchCollectionsAsync(query34);
            response34.Count.Should().Be(0);

            // Filter no result
            var query41 = new CollectionQuery()
            {
                SearchTerm = "boom",
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response41 = await sqlMandateRepository.SearchCollectionsAsync(query41);
            response41.Count.Should().Be(0);

            // Filter search term on account number OK
            var query56 = new CollectionQuery()
            {
                SearchTerm = "12345678901",
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response56 = await sqlMandateRepository.SearchCollectionsAsync(query56);
            response56.Count.Should().Be(1);
            response56[0].AccountNumber.Should().Be("12345678901");

            // Filter search term on company name
            var query58 = new CollectionQuery()
            {
                SearchTerm = "cn2",
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response58 = await sqlMandateRepository.SearchCollectionsAsync(query58);
            response58.Count.Should().Be(3);
            response58[0].AccountNumber.Should().Be("12345678903");
            response58[1].AccountNumber.Should().Be("12345678904");
            response58[2].AccountNumber.Should().Be("12345678905");

            // Filter search term on bank name
            var query45 = new CollectionQuery()
            {
                SearchTerm = "bn2",
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response45 = await sqlMandateRepository.SearchCollectionsAsync(query45);
            response45.Count.Should().Be(3);
            response45[0].AccountNumber.Should().Be("12345678902");
            response45[1].AccountNumber.Should().Be("12345678904");
            response45[2].AccountNumber.Should().Be("12345678905");

            // Filter search term on bank name
            var query59 = new CollectionQuery()
            {
                SearchTerm = "1234567890",
                SortCriteria = CollectionSortCriteria.ErpId,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response59 = await sqlMandateRepository.SearchCollectionsAsync(query59);
            response59.Count.Should().Be(2);
            response59[0].AccountNumber.Should().Be("12345678901");
            response59[1].AccountNumber.Should().Be("12345678902");

            // Filter creation date
            var query42 = new CollectionQuery()
            {
                CreationDateStart = new DateTime(2023, 9, 12, 9, 0, 0, DateTimeKind.Utc),
                CreationDateEnd = new DateTime(2023, 9, 24, 9, 0, 0, DateTimeKind.Utc),
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response42 = await sqlMandateRepository.SearchCollectionsAsync(query42);
            response42.Count.Should().Be(3);
            response42[0].AccountNumber.Should().Be("12345678902");
            response42[1].AccountNumber.Should().Be("12345678904");
            response42[2].AccountNumber.Should().Be("12345678905");

            // Filter modification date
            var query43 = new CollectionQuery()
            {
                ModificationDateStart = new DateTime(2023, 9, 12, 15, 0, 0, DateTimeKind.Utc),
                ModificationDateEnd = new DateTime(2023, 9, 29, 15, 0, 0, DateTimeKind.Utc),
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response43 = await sqlMandateRepository.SearchCollectionsAsync(query43);
            response43.Count.Should().Be(4);
            response43[0].AccountNumber.Should().Be("12345678901");
            response43[1].AccountNumber.Should().Be("12345678903");
            response43[2].AccountNumber.Should().Be("12345678904");
            response43[3].AccountNumber.Should().Be("12345678905");

            // Filter on status = 3 & 4 ok
            var query46 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 30, 40 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response46 = await sqlMandateRepository.SearchCollectionsAsync(query46);
            response46.Count.Should().Be(2);
            response46[0].AccountNumber.Should().Be("12345678902");
            response46[1].AccountNumber.Should().Be("12345678905");

            // Filter on status = 2
            var query47 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 100 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response47 = await sqlMandateRepository.SearchCollectionsAsync(query47);
            response47.Count.Should().Be(3);
            response47[0].AccountNumber.Should().Be("12345678901");
            response47[1].AccountNumber.Should().Be("12345678903");
            response47[2].AccountNumber.Should().Be("12345678904");

            // Filter on all statuses
            var query48 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 30, 40, 100 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response48 = await sqlMandateRepository.SearchCollectionsAsync(query48);
            response48.Count.Should().Be(5);

            // Filter no result
            var query49 = new CollectionQuery()
            {
                StatusCodes = new List<int> { 3 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response49 = await sqlMandateRepository.SearchCollectionsAsync(query49);
            response49.Count.Should().Be(0);
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
            CompanyDb company1 = new CompanyDb
            {
                Id = comapnyId1,
                CompanyPersonal = new CompanyPersonalDb
                {
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
                CompanyPersonal = new CompanyPersonalDb
                {
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
        public async Task SearchCollectionsAsync_Should_Filter_By_Collaborator()
        {
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            var refBank1 = EntityDbFactory.RefBankDb;
            await context.RefBank.AddAsync(refBank1);

            var refStatusCode1 = EntityDbFactory.RefStatusCodeDb;
            var refStatusCode2 = EntityDbFactory.RefStatusCodeDb;
            refStatusCode2.StatusCode = 3;
            refStatusCode2.PulseCode = 30;
            refStatusCode2.StatusNameFr = "Actif";
            await context.RefStatusCode.AddAsync(refStatusCode1);
            await context.RefStatusCode.AddAsync(refStatusCode2);

            var company1 = EntityDbFactory.CompanyDb;
            var company2 = EntityDbFactory.CompanyDb;
            var company3 = EntityDbFactory.CompanyDb;
            company2.Id = new PredictableGuid(202).NewGuid();
            company2.Name = "cn2";
            company2.ErpId = "9876543210";
            company3.Id = new PredictableGuid(212).NewGuid();
            company3.Name = "cn3";
            company3.ErpId = "9876543211";
            await context.Company.AddAsync(company1);
            await context.Company.AddAsync(company2);
            await context.Company.AddAsync(company3);

            var collaborator11 = EntityDbFactory.CollaboratorDb;
            var collaborator12 = EntityDbFactory.CollaboratorDb;
            collaborator12.Id = new PredictableGuid(204).NewGuid();
            collaborator12.Email = "collab2@email.com";
            collaborator12.FirstName = "fname2";
            collaborator12.LastName = "lname2";
            await context.Collaborator.AddAsync(collaborator11);
            await context.Collaborator.AddAsync(collaborator12);

            var companyCollaborator111 = EntityDbFactory.CompanyCollaboratorDb;
            var companyCollaborator112 = EntityDbFactory.CompanyCollaboratorDb;
            var companyCollaborator321 = EntityDbFactory.CompanyCollaboratorDb;
            companyCollaborator112.CompanyId = new PredictableGuid(202).NewGuid();
            companyCollaborator321.CompanyId = new PredictableGuid(212).NewGuid();
            companyCollaborator321.CollaboratorId = new PredictableGuid(204).NewGuid();
            await context.CompanyCollaborator.AddAsync(companyCollaborator111);
            await context.CompanyCollaborator.AddAsync(companyCollaborator112);
            await context.CompanyCollaborator.AddAsync(companyCollaborator321);

            var collection11 = EntityDbFactory.CollectionDb;
            var collection12 = EntityDbFactory.CollectionDb;
            var collection13 = EntityDbFactory.CollectionDb;
            var collection112 = EntityDbFactory.CollectionDb;
            collection12.Id = new PredictableGuid(111).NewGuid();
            collection12.CompanyId = new PredictableGuid(202).NewGuid();
            collection12.AccountNumber = "12345678902";
            collection13.Id = new PredictableGuid(121).NewGuid();
            collection13.CompanyId = new PredictableGuid(212).NewGuid();
            collection13.AccountNumber = "12345678903";
            collection112.Id = new PredictableGuid(131).NewGuid();
            collection112.AccountNumber = "12345678904";
            await context.Collection.AddAsync(collection11);
            await context.Collection.AddAsync(collection12);
            await context.Collection.AddAsync(collection13);
            await context.Collection.AddAsync(collection112);

            var status111 = EntityDbFactory.StatusDb;
            var status112 = EntityDbFactory.StatusDb;
            status111.IsCurrent = false;
            status112.Id = new PredictableGuid(104).NewGuid();
            status112.StatusDate = new DateTime(2023, 9, 19, 9, 0, 0, DateTimeKind.Utc);
            status112.IsCurrent = true;
            status112.StatusCode = 3;

            var status211 = EntityDbFactory.StatusDb;
            var status212 = EntityDbFactory.StatusDb;
            status211.Id = new PredictableGuid(105).NewGuid();
            status211.CollectionId = new PredictableGuid(111).NewGuid();
            status211.IsCurrent = false;
            status212.Id = new PredictableGuid(106).NewGuid();
            status212.StatusDate = new DateTime(2023, 9, 19, 9, 0, 0, DateTimeKind.Utc);
            status212.IsCurrent = true;
            status212.StatusCode = 3;
            status212.CollectionId = new PredictableGuid(111).NewGuid();

            var status311 = EntityDbFactory.StatusDb;
            var status312 = EntityDbFactory.StatusDb;
            status311.Id = new PredictableGuid(107).NewGuid();
            status311.CollectionId = new PredictableGuid(121).NewGuid();
            status311.IsCurrent = false;
            status312.Id = new PredictableGuid(108).NewGuid();
            status312.StatusDate = new DateTime(2023, 9, 19, 9, 0, 0, DateTimeKind.Utc);
            status312.IsCurrent = true;
            status312.StatusCode = 3;
            status312.CollectionId = new PredictableGuid(121).NewGuid();

            var status411 = EntityDbFactory.StatusDb;
            var status412 = EntityDbFactory.StatusDb;
            status411.Id = new PredictableGuid(109).NewGuid();
            status411.CollectionId = new PredictableGuid(131).NewGuid();
            status411.IsCurrent = false;
            status412.Id = new PredictableGuid(110).NewGuid();
            status412.StatusDate = new DateTime(2023, 9, 19, 9, 0, 0, DateTimeKind.Utc);
            status412.IsCurrent = true;
            status412.StatusCode = 3;
            status412.CollectionId = new PredictableGuid(131).NewGuid();

            await context.Status.AddAsync(status111);
            await context.Status.AddAsync(status112);
            await context.Status.AddAsync(status211);
            await context.Status.AddAsync(status212);
            await context.Status.AddAsync(status311);
            await context.Status.AddAsync(status312);
            await context.Status.AddAsync(status411);
            await context.Status.AddAsync(status412);

            await context.SaveChangesAsync();

            // Instanciate
            var sqlMandateRepository = new SqlMandateRepository(this.options);

            var query1 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(104).NewGuid(),
            };
            var response1 = await sqlMandateRepository.SearchCollectionsAsync(query1);
            response1.Count.Should().Be(3);
            response1[0].AccountNumber.Should().Be("12345678901");
            response1[1].AccountNumber.Should().Be("12345678902");
            response1[2].AccountNumber.Should().Be("12345678904");

            var query2 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = new PredictableGuid(204).NewGuid(),
            };
            var response2 = await sqlMandateRepository.SearchCollectionsAsync(query2);
            response2.Count.Should().Be(1);
            response2[0].AccountNumber.Should().Be("12345678903");
        }
    }
}
