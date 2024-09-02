// <copyright file="SqlMandateRepositoryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests.Tools;
    using Microsoft.EntityFrameworkCore;

    public class SqlMandateRepositoryTest : SqlServerTestBase
    {
        public SqlMandateRepositoryTest(SqlServerFixture sqlServerFixture)
            : base(sqlServerFixture)
        {
        }

        [Fact]
        public async Task SearchCollectionsAsync()
        {
            using var context = new MandateContext(_options);

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
            company2.Id = 202;
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
            collection12.CompanyId = 102;
            collection12.BankCode = "12346";
            collection12.AccountNumber = "12345678902";
            collection21.Id = new PredictableGuid(201).NewGuid();
            collection21.CompanyId = 202;
            collection21.BankCode = "12347";
            collection21.AccountNumber = "12345678903";
            collection22.Id = new PredictableGuid(211).NewGuid();
            collection22.CompanyId = 202;
            collection22.BankCode = "12346";
            collection22.AccountNumber = "12345678904";
            collection23.Id = new PredictableGuid(221).NewGuid();
            collection23.CompanyId = 202;
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
            companyCollaborator112.CompanyId = 202;
            await context.CompanyCollaborator.AddAsync(companyCollaborator111);
            await context.CompanyCollaborator.AddAsync(companyCollaborator112);

            await context.SaveChangesAsync();

            // Instanciate
            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Tests
            // Sort ascending
            var query11 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
            };
            var response26 = await sqlMandateRepository.SearchCollectionsAsync(query26);
            response26.Item2.Should().Be(5);
            response26.Item1.Count.Should().Be(5);
            response26.Item1[0].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(30);
            response26.Item1[1].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(40);
            response26.Item1[2].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(99);
            response26.Item1[3].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(99);
            response26.Item1[4].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(99);

            // Sort by status desc
            var querySt = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.Status,
                SortOrder = SortOrder.Descending,
                CollaboratorId = 104,
            };
            var responseSt = await sqlMandateRepository.SearchCollectionsAsync(querySt);
            responseSt.Item2.Should().Be(5);
            responseSt.Item1.Count.Should().Be(5);
            responseSt.Item1[0].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(99);
            responseSt.Item1[1].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(99);
            responseSt.Item1[2].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(99);
            responseSt.Item1[3].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(40);
            responseSt.Item1[4].Statuses.Single(s => s.IsCurrent).RefStatusCode!.PulseCode.Should().Be(30);

            // Page 0
            var query31 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
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
                CollaboratorId = 104,
            };
            var response46 = await sqlMandateRepository.SearchCollectionsAsync(query46);
            response46.Item2.Should().Be(2);
            response46.Item1.Count.Should().Be(2);
            response46.Item1[0].AccountNumber.Should().Be("12345678902");
            response46.Item1[1].AccountNumber.Should().Be("12345678905");

            // Filter on status = 2
            var query47 = new CollectionQuery()
            {
                StatusCodes = new List<int> { (int)JdcCollectionStatus.Creation_InProgress },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = 104,
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
                StatusCodes = new List<int> { 30, 40, 99 },
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = 104,
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
                CollaboratorId = 104,
            };
            var response49 = await sqlMandateRepository.SearchCollectionsAsync(query49);
            response49.Item2.Should().Be(0);
            response49.Item1.Count.Should().Be(0);

            var response50 = await sqlMandateRepository.SearchCollectionsAsync(query11);
            response50.Item2.Should().Be(5);
            response50.Item1.Count.Should().Be(5);
            response50.Item1[0].AccountNumber.Should().Be("12345678901");
            response50.Item1[1].AccountNumber.Should().Be("12345678902");
            response50.Item1[2].AccountNumber.Should().Be("12345678903");
            response50.Item1[3].AccountNumber.Should().Be("12345678904");
            response50.Item1[4].AccountNumber.Should().Be("12345678905");
        }

        [Fact]
        public async Task GetAllCompaniesByCollaboratorAsync()
        {
            // Arrange
            using var context = new MandateContext(_options);
            var sqlMandateRepository = new SqlMandateRepository(_options);

            PredictableGuid generator = new PredictableGuid();
            int comapnyId1 = 1;
            int comapnyId2 = 2;
            int collaboratorId = 1;

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
                IsActive = true,
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
                IsActive = true,
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
        public async Task GetAllCompaniesByCollaboratorAsyncShouldReturnOnlyActive()
        {
            // Arrange

            using var context = new MandateContext(_options);
            var sqlMandateRepository = new SqlMandateRepository(_options);

            PredictableGuid generator = new PredictableGuid();
            int comapnyId1 = 1;
            int comapnyId2 = 2;
            int collaboratorId = 1;

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
                IsActive = true,
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
                IsActive = false,
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
            companies.Count.Should().Be(1);
            companies[0]?.Name.Should().Be("Microsoft");
            companies[0]?.SiretNumber.Should().Be("40902900600031");
            companies[0]?.ErpId.Should().Be("1000265308");
        }

        [Fact]
        public async Task SearchCollectionsAsync_Should_Filter_By_Collaborator()
        {

            using var context = new MandateContext(_options);

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
            company2.Id = 202;
            company2.Name = "cn2";
            company2.ErpId = "9876543210";
            company3.Id = 212;
            company3.Name = "cn3";
            company3.ErpId = "9876543211";
            await context.Company.AddAsync(company1);
            await context.Company.AddAsync(company2);
            await context.Company.AddAsync(company3);

            var collaborator11 = EntityDbFactory.CollaboratorDb;
            var collaborator12 = EntityDbFactory.CollaboratorDb;
            collaborator12.Id = 204;
            collaborator12.Email = "collab2@email.com";
            collaborator12.FirstName = "fname2";
            collaborator12.LastName = "lname2";
            await context.Collaborator.AddAsync(collaborator11);
            await context.Collaborator.AddAsync(collaborator12);

            var companyCollaborator111 = EntityDbFactory.CompanyCollaboratorDb;
            var companyCollaborator112 = EntityDbFactory.CompanyCollaboratorDb;
            var companyCollaborator321 = EntityDbFactory.CompanyCollaboratorDb;
            companyCollaborator112.CompanyId = 202;
            companyCollaborator321.CompanyId = 212;
            companyCollaborator321.CollaboratorId = 204;
            await context.CompanyCollaborator.AddAsync(companyCollaborator111);
            await context.CompanyCollaborator.AddAsync(companyCollaborator112);
            await context.CompanyCollaborator.AddAsync(companyCollaborator321);

            var collection11 = EntityDbFactory.CollectionDb;
            var collection12 = EntityDbFactory.CollectionDb;
            var collection13 = EntityDbFactory.CollectionDb;
            var collection112 = EntityDbFactory.CollectionDb;
            collection12.Id = new PredictableGuid(111).NewGuid();
            collection12.CompanyId = 202;
            collection12.AccountNumber = "12345678902";
            collection13.Id = new PredictableGuid(121).NewGuid();
            collection13.CompanyId = 212;
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
            var sqlMandateRepository = new SqlMandateRepository(_options);

            var query1 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = 104,
            };
            var response1 = await sqlMandateRepository.SearchCollectionsAsync(query1);
            response1.Item2.Should().Be(3);
            response1.Item1.Count.Should().Be(3);
            response1.Item1[0].AccountNumber.Should().Be("12345678901");
            response1.Item1[1].AccountNumber.Should().Be("12345678902");
            response1.Item1[2].AccountNumber.Should().Be("12345678904");

            var query2 = new CollectionQuery()
            {
                SortCriteria = CollectionSortCriteria.AccountNumber,
                SortOrder = SortOrder.Ascending,
                CollaboratorId = 204,
            };
            var response2 = await sqlMandateRepository.SearchCollectionsAsync(query2);
            response2.Item2.Should().Be(1);
            response2.Item1.Count.Should().Be(1);
            response2.Item1[0].AccountNumber.Should().Be("12345678903");
        }

        [Fact]
        public async Task GetCollectionById()
        {

            using var context = new MandateContext(_options);

            // GetCollectionById
            var collectionId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
            var companyId = 101;
            var companyId2 = 102;

            var collection = new CollectionDb()
            {
                Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                CompanyId = 101,
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
                CompanyId = 101,
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

            var sqlMandateRepository = new SqlMandateRepository(_options);

            var result = await sqlMandateRepository.GetCollectionById(collectionId);

            result.Should().NotBeNull();
            result.Id.Should().Be(Guid.Parse("a1111111-1111-1111-1111-111111111111"));
        }

        [Fact]
        public async Task GetCollectionById_WhenCollectionNotFound_ShouldThrowException()
        {

            using var context = new MandateContext(_options);

            // GetCollectionById
            var collectionId = Guid.Parse("a1111111-1111-1111-1111-111111111111");
            var sqlMandateRepository = new SqlMandateRepository(_options);

            Func<Task> act = async () => await sqlMandateRepository.GetCollectionById(collectionId);
            await act.Should().ThrowExactlyAsync<CollectionNotFoundException>()
                .WithMessage("La collecton avec l'id 'a1111111-1111-1111-1111-111111111111' n'a pas été trouvée");
        }

        [Fact]
        public async Task SaveSignatoryAsync_CaseNewEntity()
        {

            using var context = new MandateContext(_options);

            var companyId = 1;
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

            var sqlMandateRepository = new SqlMandateRepository(_options);

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
            var db = _sqlServerFixture.ExecuteQuery("select * from [Mandate].[Personal]");

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

            var sqlMandateRepository = new SqlMandateRepository(_options);
            await sqlMandateRepository.CreateFakeRefAsync();
            await sqlMandateRepository.AddFakeDataAsync();
            await sqlMandateRepository.CreateFakeAuthAsync();

            using var context = new MandateContext(_options);
            context.RefPdfTemplate.Count().Should().Be(1);
            context.RefBank.Count().Should().Be(10 + 194);
            context.RefBank.Count(b => b.JdcPartnership == JdcPartnership.NonPartner).Should().Be(1 + 4);
            context.RefBank.Count(b => b.JdcPartnership == JdcPartnership.Scrappable).Should().Be(1 + 70);
            context.RefStatusCode.Count().Should().Be(20);
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
            context.RefBank.Count().Should().Be(194);
            context.RefStatusCode.Count().Should().Be(0);
        }

        public async Task AddCollaboratorFakeData(MandateContext context, int collaboratorId, string userEmail, int companyId, bool collabIsActive = true)
        {
            var collabDb = new CollaboratorDb()
            {
                Id = collaboratorId,
                Email = userEmail,
                IsActive = collabIsActive,
            };

            await context.Collaborator.AddAsync(collabDb);

            var ccDb = new CompanyCollaboratorDb()
            {
                CollaboratorId = collaboratorId,
                CompanyId = companyId,
            };

            await context.CompanyCollaborator.AddAsync(ccDb);
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_ShouldReturnCompany_WhenCompanyExists()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();

            // Ensure this is the same ID used for the foreign key in CollectionDb
            int companyId = 101;
            int collaboratorId = 651;
            var erpId = "validErpId";
            var userEmail = "user@email.test";

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

            var personalDB = new PersonalDb()
            {
                FirstName = "firstname",
                LastName = "lastname",
                Email = "email",
            };

            var expectedCompany = new CompanyDb
            {
                Id = companyId,
                Name = "Dior",
                SiretNumber = "40930900600031",
                ErpId = erpId,
                IsActive = true,
                Personal = personalDB,
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

            await this.AddCollaboratorFakeData(context, collaboratorId, userEmail, companyId);

            await context.Collection.AddAsync(collectionDb);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act
            var result = await sqlMandateRepository.GetCompanyByErpIdAsync(erpId, userEmail);

            // Assert
            result.Id.Should().Be(expectedCompany.Id);
            result.ErpId.Should().Be(expectedCompany.ErpId);
            result.SiretNumber.Should().Be(expectedCompany.SiretNumber);
            result.Name.Should().Be(expectedCompany.Name);
            result.Personal?.Email.Should().BeEquivalentTo(expectedCompany.Personal.Email);
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_ShouldThrowCompanyNotFoundException_WhenCompanyDoesNotExist()
        {
            // Arrange

            using var context = new MandateContext(_options);

            var sqlMandateRepository = new SqlMandateRepository(_options);
            var nonExistingErpId = "nonExistingErpId";
            var userEmail = "user@email.test";

            // Act & Assert
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyByErpIdAsync(nonExistingErpId, userEmail);

            await act.Should().ThrowAsync<CompanyNotFoundException>();
        }

        [Fact]
        public async Task GetCollaboratorByEmail()
        {

            using var context = new MandateContext(_options);

            var collab = EntityDbFactory.CollaboratorDb;
            var collab2 = EntityDbFactory.CollaboratorDb;
            collab2.Id = 105;
            collab2.Email = "collab2@email.com";
            collab2.IsActive = true;
            await context.Collaborator.AddAsync(collab);
            await context.Collaborator.AddAsync(collab2);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);
            var res = await sqlMandateRepository.GetCollaboratorByEmailAsync("collab@email.com");

            res.Id.Should().Be(104);
            res.Email.Should().Be("collab@email.com");
            res.FirstName.Should().Be("fname");
            res.LastName.Should().Be("lname");
        }

        [Fact]
        public async Task GetCollaboratorByEmail_ShouldNotRetunWhenIsNotActive()
        {

            using var context = new MandateContext(_options);

            var collab = EntityDbFactory.CollaboratorDb;
            collab.IsActive = false;
            var collab2 = EntityDbFactory.CollaboratorDb;
            collab2.Id = 105;
            collab2.Email = "collab2@email.com";
            collab2.IsActive = false;
            await context.Collaborator.AddAsync(collab);
            await context.Collaborator.AddAsync(collab2);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            var res = await sqlMandateRepository.GetCollaboratorByEmailAsync("collab@email.com");

            res.Should().BeNull();
        }

        [Fact]
        public async Task GetActiveContactByIdAsync_ShouldNotRetunWhenIsNotActive()
        {

            using var context = new MandateContext(_options);

            var collab = EntityDbFactory.CollaboratorDb;
            var collab2 = EntityDbFactory.CollaboratorDb;
            collab2.Id = 105;
            collab2.Email = "collab2@email.com";
            collab2.IsActive = false;
            await context.Collaborator.AddAsync(collab);
            await context.Collaborator.AddAsync(collab2);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);
            var res = await sqlMandateRepository.GetActiveContactByIdAsync(105);

            res.Should().BeNull();
        }

        [Fact]
        public async Task GetContactByIdAsync_ShouldRetunWhenIsNotActive()
        {

            using var context = new MandateContext(_options);

            var collab = EntityDbFactory.CollaboratorDb;
            var collab2 = EntityDbFactory.CollaboratorDb;
            collab2.Id = 105;
            collab2.Email = "collab2@email.com";
            collab2.IsActive = false;
            await context.Collaborator.AddAsync(collab);
            await context.Collaborator.AddAsync(collab2);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);
            var res = await sqlMandateRepository.GetContactByIdAsync(105);

            res.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_WithContactId()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();

            // Ensure this is the same ID used for the foreign key in CollectionDb
            int companyId = 101;
            int collaboratorId = 651;
            var erpId = "validErpId";
            var userEmail = "user@email.test";

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

            var personalDB = new PersonalDb()
            {
                FirstName = "firstname",
                LastName = "lastname",
                Email = "email",
            };

            var expectedCompany = new CompanyDb
            {
                Id = companyId,
                Name = "Dior",
                SiretNumber = "40930900600031",
                ErpId = erpId,
                IsActive = true,
                Personal = personalDB,
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

            await this.AddCollaboratorFakeData(context, collaboratorId, userEmail, companyId);

            await context.Collection.AddAsync(collectionDb);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act
            var result = await sqlMandateRepository.GetCompanyByErpIdAsync(erpId, collaboratorId);

            // Assert
            result.Id.Should().Be(expectedCompany.Id);
            result.ErpId.Should().Be(expectedCompany.ErpId);
            result.SiretNumber.Should().Be(expectedCompany.SiretNumber);
            result.Name.Should().Be(expectedCompany.Name);
            result.Personal?.Email.Should().BeEquivalentTo(expectedCompany.Personal.Email);
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_WithContactId_WhenCompanyDoesNotExist()
        {
            // Arrange

            using var context = new MandateContext(_options);

            var sqlMandateRepository = new SqlMandateRepository(_options);
            var nonExistingErpId = "nonExistingErpId";

            // Act & Assert
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyByErpIdAsync(nonExistingErpId, 12);

            await act.Should().ThrowAsync<CompanyNotFoundException>();
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_WithContactId_WhenCollaboratorIsInactive()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();
            int companyId = 1;  // Ensure this is the same ID used for the foreign key in CollectionDb
            var erpId = "validErpId";
            int collaboratorId = 652;
            var userEmail = "user@email.test";
            var siretNumber = "40930900600031";

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
                SiretNumber = siretNumber,
                ErpId = erpId,
                IsActive = true,
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

            await this.AddCollaboratorFakeData(context, collaboratorId, userEmail, companyId, false);
            await context.Collection.AddAsync(collectionDb);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act & Assert
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyByErpIdAsync(erpId, collaboratorId);

            await act.Should().ThrowAsync<InaccessibleCompanyException>();
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_WithContactId_WhenCompanyExistsAndIsNotInPortfolio()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();
            int companyId = 1;
            int otherCompanyId = 14323;
            var erpId = "validErpId";
            int collaboratorId = 652;
            var userEmail = "user@email.test";
            var siretNumber = "40930900600031";

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
                SiretNumber = siretNumber,
                ErpId = erpId,
                IsActive = true,
            };

            await context.Company.AddAsync(expectedCompany);

            var otherCompany = new CompanyDb
            {
                Id = otherCompanyId,
                Name = "Other",
                SiretNumber = siretNumber,
                ErpId = "0123456789",
                IsActive = true,
            };
            await context.Company.AddAsync(otherCompany);

            await context.SaveChangesAsync();

            Guid collectionId = generator.NewGuid();
            var collectionDb = new CollectionDb()
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

            await this.AddCollaboratorFakeData(context, collaboratorId, userEmail, otherCompanyId);
            await context.Collection.AddAsync(collectionDb);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act & Assert
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyByErpIdAsync(erpId, collaboratorId);

            await act.Should().ThrowAsync<InaccessibleCompanyException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetCollaboratorByEmail_WhenNoCollab(bool persistCollab)
        {

            using var context = new MandateContext(_options);

            if (persistCollab)
            {
                var collab = EntityDbFactory.CollaboratorDb;
                await context.Collaborator.AddAsync(collab);
                await context.SaveChangesAsync();
            }

            var sqlMandateRepository = new SqlMandateRepository(_options);
            var res = await sqlMandateRepository.GetCollaboratorByEmailAsync("collaborator@email.com");

            res.Should().BeNull();
        }

        [Fact]
        public async Task CreateOrUpdateFolderAsync_Case_Create()
        {

            using var context = new MandateContext(_options);

            var company1 = EntityDbFactory.CompanyDb;
            await context.Company.AddAsync(company1);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            await sqlMandateRepository.CreateOrUpdateFolderAsync("folderId2", 102);

            var companyFolder = await context.JeDeclareFolder
                .Where(item => item.CompanyId == 102)
                .ToListAsync();

            companyFolder.Count.Should().Be(1);
            companyFolder.Single().CompanyId.Should().Be(102);
            companyFolder.Single().JdcDossierId.Should().Be("folderId2");
        }

        [Fact]
        public async Task CreateOrUpdateFolderAsync_Case_Update()
        {

            using var context = new MandateContext(_options);

            var company1 = EntityDbFactory.CompanyDb;
            await context.Company.AddAsync(company1);
            await context.SaveChangesAsync();

            var jdcFolder = EntityDbFactory.JeDeclareFolderDb;
            await context.JeDeclareFolder.AddAsync(jdcFolder);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            await sqlMandateRepository.CreateOrUpdateFolderAsync("folderId", 102);
            var all = await context.JeDeclareFolder.ToListAsync();
            var companyFolder = await context.JeDeclareFolder
                .Where(item => item.CompanyId == 102)
                .ToListAsync();

            companyFolder.Count.Should().Be(1);
            companyFolder.Single().CompanyId.Should().Be(102);
            companyFolder.Single().JdcDossierId.Should().Be("folderId");
        }

        [Fact]
        public async Task GetCompanyByErpIdSiretAsync_WhenErpIdSiretMatchOnes_ShouldReturnCompany()
        {

            using var context = new MandateContext(_options);
            CompanyDb company = EntityDbFactory.CompanyDb;
            CompanyDb company2 = EntityDbFactory.CompanyDb;
            company2.Id = 103;
            company2.SiretNumber = "12345678901220";

            await context.AddRangeAsync(new List<CompanyDb> { company, company2 });
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            var dbCompany = await sqlMandateRepository.GetCompanyByErpIdSiretAsync("1234567890", "12345678901234");

            dbCompany.ErpId.Should().Be("1234567890");
            dbCompany.SiretNumber.Should().Be("12345678901234");
        }

        [Fact]
        public async Task GetCompanyByErpIdSiretAsync_WhenErpIdSiretMatchTwice_ShouldThrowException()
        {

            using var context = new MandateContext(_options);
            CompanyDb company = EntityDbFactory.CompanyDb;
            CompanyDb company2 = EntityDbFactory.CompanyDb;
            CompanyDb company3 = EntityDbFactory.CompanyDb;
            company2.Id = 103;
            company3.Id = 104;
            company3.SiretNumber = "12345678901220";

            await context.AddRangeAsync(new List<CompanyDb> { company, company2, company3 });
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            Func<Task> func = async () => await sqlMandateRepository.GetCompanyByErpIdSiretAsync("1234567890", "12345678901234");
            var exception = await func.Should().ThrowExactlyAsync<CustomCompanyNotFoundException>();
            exception.And.Type.Should().Be(ExceptionType.AccountNumberMatchDoubleSiret);
        }

        [Fact]
        public async Task GetCompanyByErpIdSiretAsync_WhenErpIdNoSiretMatch_ShouldThrowException()
        {

            using var context = new MandateContext(_options);
            CompanyDb company = EntityDbFactory.CompanyDb;
            CompanyDb company2 = EntityDbFactory.CompanyDb;
            CompanyDb company3 = EntityDbFactory.CompanyDb;
            company2.Id = 103;
            company3.Id = 104;
            company3.SiretNumber = "12345678901220";

            await context.AddRangeAsync(new List<CompanyDb> { company, company2, company3 });
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            Func<Task> func = async () => await sqlMandateRepository.GetCompanyByErpIdSiretAsync("1234567890", "12345678901230");
            var exception = await func.Should().ThrowExactlyAsync<CustomCompanyNotFoundException>();
            exception.And.Type.Should().Be(ExceptionType.AccountNumberNoMatchSiret);
        }

        [Fact]
        public async Task GetCompanyByErpIdSiretAsync_WhenNonExistingErpIdSiretMatch()
        {

            using var context = new MandateContext(_options);

            await context.AddRangeAsync(GenerateCompanies());
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            var dbCompany = await sqlMandateRepository.GetCompanyByErpIdSiretAsync("1234567800", "12345678901220");

            dbCompany.ErpId.Should().Be("1234567890");
            dbCompany.SiretNumber.Should().Be("12345678901220");
        }

        [Fact]
        public async Task GetCompanyByErpIdSiretAsync_WhenNonExistingErpIdNoSiretMatch_shouldThrowException()
        {

            using var context = new MandateContext(_options);

            await context.AddRangeAsync(GenerateCompanies());
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);
            Func<Task> func = async () => await sqlMandateRepository.GetCompanyByErpIdSiretAsync("1234567800", "12345678901240");
            var exception = await func.Should().ThrowExactlyAsync<CustomCompanyNotFoundException>();
            exception.And.Type.Should().Be(ExceptionType.NoAccountNumberNoMatchSiret);
        }

        [Fact]
        public async Task GetCompanyByErpIdSiretAsync_WhenNonExistingErpIdSiretMatchMoreThenOnce_shouldThrowException()
        {

            using var context = new MandateContext(_options);

            await context.AddRangeAsync(GenerateCompanies());
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);
            Func<Task> func = async () => await sqlMandateRepository.GetCompanyByErpIdSiretAsync("1234567800", "12345678901234");
            var exception = await func.Should().ThrowExactlyAsync<CustomCompanyNotFoundException>();
            exception.And.Type.Should().Be(ExceptionType.NoAccountNumberMatchDoubleSiret);
        }

        [Fact]
        public async Task GetCompanyByErpIdSiretAsync_ShouldThrowWhenNoCompanyFound()
        {

            using var context = new MandateContext(_options);

            await context.AddRangeAsync(GenerateCompanies());
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            Func<Task> func = async () => await sqlMandateRepository.GetCompanyByErpIdSiretAsync("1234567800", "98765432101240");

            var exception = await func.Should().ThrowExactlyAsync<CustomCompanyNotFoundException>();
            exception.And.Type.Should().Be(ExceptionType.NoAccountNumberNoMatchSiret);
        }

        [Fact]
        public async Task InsertFormIOCollectionAsync()
        {
            // Arrange
            var collection = EntityDbFactory.CollectionDb;
            collection.Bank = EntityDbFactory.RefBankDb;
            collection.Company = EntityDbFactory.CompanyDb;
            collection.Company.JeDeclareFolder = EntityDbFactory.JeDeclareFolderDb;
            collection.JeDeclareCollection = EntityDbFactory.JeDeclareCollectionDb;
            collection.Personal = EntityDbFactory.PersonalDb;
            collection.Statuses = EntityDbFactory.Statuses;


            using var context = new MandateContext(_options);

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act
            await sqlMandateRepository.InsertFormIOCollectionAsync(collection);

            // Assert
            var dbCollection = await context.Collection
                                            .Include(_ => _.Bank)
                                            .Include(_ => _.Company)
                                            .Include(_ => _.JeDeclareCollection)
                                            .Include(_ => _.Personal)
                                            .Include(_ => _.Statuses)
                                            .FirstOrDefaultAsync(_ => _.Id == collection.Id);

            dbCollection.Should().NotBeNull();
            dbCollection!.Id.Should().Be(collection.Id);
            dbCollection!.Bank!.BankCode.Should().Be(collection.Bank!.BankCode);
            dbCollection!.Company!.Id.Should().Be(collection.Company!.Id);
            dbCollection!.JeDeclareCollection!.Id.Should().Be(collection.JeDeclareCollection!.Id);
            dbCollection!.Personal!.Id.Should().Be(collection.Personal!.Id);
            dbCollection!.Statuses[0].Id!.Should().Be(collection.Statuses[0].Id);
            dbCollection!.Statuses.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetCompanyBySiretAsync_ShouldReturnCompany_WhenCompanyExists()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();
            int companyId = 1;  // Ensure this is the same ID used for the foreign key in CollectionDb
            var erpId = "validErpId";
            var siretNumber = "40930900600031";

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
                SiretNumber = siretNumber,
                ErpId = erpId,
                IsActive = true,
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

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act
            var result = await sqlMandateRepository.GetCompanyBySiretAsync(siretNumber);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(expectedCompany.Id);
            result!.ErpId.Should().Be(expectedCompany.ErpId);
            result!.SiretNumber.Should().Be(expectedCompany.SiretNumber);
            result!.Name.Should().Be(expectedCompany.Name);
        }

        [Fact]
        public async Task GetCompanyBySiretAsync_ShouldNotReturnCompany_WhenCompanyExistsAndIsNotActive()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();
            int companyId = 1;  // Ensure this is the same ID used for the foreign key in CollectionDb
            var erpId = "validErpId";
            var siretNumber = "40930900600031";

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
                SiretNumber = siretNumber,
                ErpId = erpId,
                IsActive = false,
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

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyBySiretAsync(siretNumber);

            // Assert
            await act.Should().ThrowAsync<CompanyNotFoundException>();
        }

        [Fact]
        public async Task GetCompanyByErpIdSiretAsync_ShouldNotReturnCompany_WhenCompanyExistsAndIsNotActive()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();
            int companyId = 1;  // Ensure this is the same ID used for the foreign key in CollectionDb
            var erpId = "validErpId";
            var siretNumber = "40930900600031";

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
                SiretNumber = siretNumber,
                ErpId = erpId,
                IsActive = false,
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

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyByErpIdSiretAsync(erpId, siretNumber);

            // Assert
            await act.Should().ThrowAsync<CustomCompanyNotFoundException>();
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_ShouldNotReturnCompany_WhenCompanyExistsAndIsNotInPortfolio()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();
            int companyId = 1;
            int otherCompanyId = 14323;
            var erpId = "validErpId";
            int collaboratorId = 652;
            var userEmail = "user@email.test";
            var siretNumber = "40930900600031";

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
                SiretNumber = siretNumber,
                ErpId = erpId,
                IsActive = true,
            };

            await context.Company.AddAsync(expectedCompany);

            var otherCompany = new CompanyDb
            {
                Id = otherCompanyId,
                Name = "Other",
                SiretNumber = siretNumber,
                ErpId = "0123456789",
                IsActive = true,
            };
            await context.Company.AddAsync(otherCompany);

            await context.SaveChangesAsync();

            Guid collectionId = generator.NewGuid();
            var collectionDb = new CollectionDb()
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

            await this.AddCollaboratorFakeData(context, collaboratorId, userEmail, otherCompanyId);
            await context.Collection.AddAsync(collectionDb);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act & Assert
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyByErpIdAsync(erpId, userEmail);

            await act.Should().ThrowAsync<InaccessibleCompanyException>();
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_ShouldNotReturnCompany_WhenCollaboratorIsInactive()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();
            int companyId = 1;  // Ensure this is the same ID used for the foreign key in CollectionDb
            var erpId = "validErpId";
            int collaboratorId = 652;
            var userEmail = "user@email.test";
            var siretNumber = "40930900600031";

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
                SiretNumber = siretNumber,
                ErpId = erpId,
                IsActive = true,
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

            await this.AddCollaboratorFakeData(context, collaboratorId, userEmail, companyId, false);
            await context.Collection.AddAsync(collectionDb);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act & Assert
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyByErpIdAsync(erpId, userEmail);

            await act.Should().ThrowAsync<InaccessibleCompanyException>();
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_ShouldNotReturnCompany_WhenCompanyExistsAndIsNotActive()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();
            int companyId = 1;  // Ensure this is the same ID used for the foreign key in CollectionDb
            var erpId = "validErpId";
            int collaboratorId = 652;
            var userEmail = "user@email.test";
            var siretNumber = "40930900600031";

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
                SiretNumber = siretNumber,
                ErpId = erpId,
                IsActive = false,
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

            await this.AddCollaboratorFakeData(context, collaboratorId, userEmail, companyId);
            await context.Collection.AddAsync(collectionDb);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act & Assert
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyByErpIdAsync(erpId, userEmail);

            await act.Should().ThrowAsync<InactiveCompanyException>();
        }

        [Fact]
        public async Task GetAccountByIdAsync_ShouldReturnCompany_WhenCompanyExistsAndIsNotActive()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();
            int companyId = 1;  // Ensure this is the same ID used for the foreign key in CollectionDb
            var erpId = "validErpId";
            var siretNumber = "40930900600031";

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
                SiretNumber = siretNumber,
                ErpId = erpId,
                IsActive = false,
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

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act
            var result = await sqlMandateRepository.GetAccountByIdAsync(companyId);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetActiveAccountByIdAsync_ShouldNotReturnCompany_WhenCompanyExistsAndIsNotActive()
        {
            // Arrange

            using var context = new MandateContext(_options);

            PredictableGuid generator = new PredictableGuid();
            int companyId = 1;  // Ensure this is the same ID used for the foreign key in CollectionDb
            var erpId = "validErpId";
            var siretNumber = "40930900600031";

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
                SiretNumber = siretNumber,
                ErpId = erpId,
                IsActive = false,
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

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act
            var result = await sqlMandateRepository.GetActiveAccountByIdAsync(companyId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCompanyBySiretAsync_Should_Throws_Exception_WhenCompanyDoNotExists()
        {
            // Arrange

            using var context = new MandateContext(_options);

            var sqlMandateRepository = new SqlMandateRepository(_options);

            // Act
            Func<Task> act = async () => await sqlMandateRepository.GetCompanyBySiretAsync("12345");

            // Assert
            await act.Should().ThrowAsync<CompanyNotFoundException>();
        }

        [Fact]
        public async Task GetPdfTemplateByCodeAsync()
        {

            using var context = new MandateContext(_options);

            var bank = EntityDbFactory.RefBankDb;
            await context.RefBank.AddAsync(bank);
            await context.SaveChangesAsync();

            var template = EntityDbFactory.RefPdfTemplateDb;
            await context.RefPdfTemplate.AddAsync(template);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            var res = await sqlMandateRepository.GetPdfTemplateByCodeAsync("12345");
            res.Should().BeEquivalentTo(Convert.FromBase64String("dGVzdA=="));
        }

        [Fact]
        public async Task GetPdfTemplateByCodeAsync_WhenBankNotFound_ShouldThrowException()
        {

            using var context = new MandateContext(_options);

            var bank = EntityDbFactory.RefBankDb;
            await context.RefBank.AddAsync(bank);
            await context.SaveChangesAsync();

            var template = EntityDbFactory.RefPdfTemplateDb;
            await context.RefPdfTemplate.AddAsync(template);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            Func<Task> act = async () => await sqlMandateRepository.GetPdfTemplateByCodeAsync("67890");
            await act.Should().ThrowExactlyAsync<BankCodeNotFoundException>()
                .WithMessage("La banque avec le code '67890' n'a pas été trouvée dans le référentiel");
        }

        [Fact]
        public async Task UpdateCurrentStatusAsync_WhenStatusNotFound_ShouldThrowException()
        {

            using var context = new MandateContext(_options);
            var sqlMandateRepository = new SqlMandateRepository(_options);

            Func<Task> act = async () => await sqlMandateRepository.UpdateCurrentStatusAsync(Guid.Parse("a1111111-1111-1111-1111-111111111111"));
            await act.Should().ThrowExactlyAsync<StatusNotFoundException>()
                .WithMessage("La collection avec l\'id 'a1111111-1111-1111-1111-111111111111' n'a pas de status en cours");
        }

        [Fact]
        public async Task GetRefBankByCodeAsync_WhenBankNotFound_ShouldThrowException()
        {

            using var context = new MandateContext(_options);
            var sqlMandateRepository = new SqlMandateRepository(_options);

            Func<Task> act = async () => await sqlMandateRepository.GetRefBankByCodeAsync("11111");
            await act.Should().ThrowExactlyAsync<BankCodeNotFoundException>()
                .WithMessage("La banque avec le code '11111' n'a pas été trouvée dans le référentiel");
        }

        [Fact]
        public async Task GetRefStatusCodeByJdcCodeAsync_CaseOk()
        {

            using var context = new MandateContext(_options);

            var refStatusCode = EntityDbFactory.RefStatusCodeDb;
            await context.RefStatusCode.AddAsync(refStatusCode);
            await context.SaveChangesAsync();

            var refBank = EntityDbFactory.RefBankDb;
            await context.RefBank.AddAsync(refBank);
            await context.SaveChangesAsync();

            var company = EntityDbFactory.CompanyDb;
            await context.Company.AddAsync(company);
            await context.SaveChangesAsync();

            var collection = EntityDbFactory.CollectionDb;
            await context.Collection.AddAsync(collection);
            await context.SaveChangesAsync();

            var status = EntityDbFactory.StatusDb;
            await context.Status.AddAsync(status);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            var statusResult = await sqlMandateRepository.GetRefStatusCodeByJdcCodeAsync("99");

            statusResult.RefStatusCode!.StatusCode.Should().Be(99);
            statusResult.RefStatusCode!.PulseCode.Should().Be(99);
        }

        [Fact]
        public async Task GetRefStatusCodeByJdcCodeAsync_ThrowStatusNotFound_CaseStatusCodeNotFound()
        {

            using var context = new MandateContext(_options);

            var refStatusCode = EntityDbFactory.RefStatusCodeDb;
            await context.RefStatusCode.AddAsync(refStatusCode);
            await context.SaveChangesAsync();

            var refBank = EntityDbFactory.RefBankDb;
            await context.RefBank.AddAsync(refBank);
            await context.SaveChangesAsync();

            var company = EntityDbFactory.CompanyDb;
            await context.Company.AddAsync(company);
            await context.SaveChangesAsync();

            var collection = EntityDbFactory.CollectionDb;
            await context.Collection.AddAsync(collection);
            await context.SaveChangesAsync();

            var status = EntityDbFactory.StatusDb;
            await context.Status.AddAsync(status);
            await context.SaveChangesAsync();

            var sqlMandateRepository = new SqlMandateRepository(_options);

            Func<Task> act = async () => await sqlMandateRepository.GetRefStatusCodeByJdcCodeAsync("-11");

            await act.Should().ThrowAsync<StatusNotFoundException>();
        }

        private static List<CompanyDb> GenerateCompanies()
        {
            CompanyDb company = EntityDbFactory.CompanyDb;
            CompanyDb company2 = EntityDbFactory.CompanyDb;
            CompanyDb company3 = EntityDbFactory.CompanyDb;
            CompanyDb company4 = EntityDbFactory.CompanyDb;
            company2.Id = 103;
            company2.SiretNumber = "12345678901220";
            company3.Id = 104;
            company3.ErpId = "1234567891";
            company4.Id = 105;

            return new List<CompanyDb>() { company, company2, company3, company4 };
        }

        [Fact]
        public async Task CreateContactByEventAsync_ShouldCreateContact_WhenEventIsValid()
        {
            // Arrange
            var validEvent = EntityDbFactory.CollaboratorDb;

            using var context = new MandateContext(_options);

            // Act
            var sqlMandateRepository = new SqlMandateRepository(_options);
            await sqlMandateRepository.CreateContactByEventAsync(validEvent);

            // Assert
            context.Collaborator.Count().Should().Be(1);
            context.Collaborator.First().Id.Should().Be(104);
        }

        [Fact]
        public async Task CreateContactByEventAsync_ShouldThrowArgumentNullException_WhenEventIsNull()
        {
            // Arrange
            CollaboratorDb nullContact = null!;

            using var context = new MandateContext(_options);

            // Act
            var sqlMandateRepository = new SqlMandateRepository(_options);
            Func<Task> act = async () => await sqlMandateRepository.CreateContactByEventAsync(nullContact);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdateContactByEventAsync_ShouldCreateContact_WhenEventIsValid()
        {
            // Arrange
            var validEvent = EntityDbFactory.CollaboratorDb;

            using var context = new MandateContext(_options);

            // Add with IsActive at true
            await context.Collaborator.AddAsync(validEvent);
            await context.SaveChangesAsync();

            // Act
            validEvent.IsActive = false;
            var sqlMandateRepository = new SqlMandateRepository(_options);
            await sqlMandateRepository.UpdateContactByEventAsync(validEvent);

            // Assert
            context.Collaborator.Count().Should().Be(1);
            context.Collaborator.First().IsActive.Should().Be(false);
        }

        [Fact]
        public async Task UpdateContactByEventAsync_ShouldThrowArgumentNullException_WhenEventIsNull()
        {
            // Arrange
            CollaboratorDb nullContact = null!;

            using var context = new MandateContext(_options);

            // Act
            var sqlMandateRepository = new SqlMandateRepository(_options);
            Func<Task> act = async () => await sqlMandateRepository.UpdateContactByEventAsync(nullContact);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task CreateAccountByEventAsync_ShouldCreateAccount_WhenEventIsValid()
        {
            // Arrange
            var validEvent = EntityDbFactory.CompanyDb;

            using var context = new MandateContext(_options);

            // Act
            var sqlMandateRepository = new SqlMandateRepository(_options);
            await sqlMandateRepository.CreateCompanyAsync(validEvent);

            // Assert
            context.Company.Count().Should().Be(1);
            context.Company.First().Id.Should().Be(102);
        }

        [Fact]
        public async Task UpdateAccountByEventAsync_ShouldCreateAccount_WhenEventIsValid()
        {
            // Arrange
            var validEvent = EntityDbFactory.CompanyDb;

            using var context = new MandateContext(_options);

            // Add with IsActive at true
            await context.Company.AddAsync(validEvent);
            await context.SaveChangesAsync();

            // Act
            validEvent.IsActive = false;
            var sqlMandateRepository = new SqlMandateRepository(_options);
            await sqlMandateRepository.UpdateCompanyAsync(validEvent);

            // Assert
            context.Company.Count().Should().Be(1);
            context.Company.First().IsActive.Should().Be(false);
        }

        [Fact]
        public async Task CreateRoleByEventAsync_ShouldCreateRole_WhenEventIsValid()
        {
            // Arrange
            var validEvent = EntityDbFactory.CompanyDb;
            var validContact = EntityDbFactory.CollaboratorDb;

            using var context = new MandateContext(_options);

            await context.Company.AddAsync(validEvent);
            await context.Collaborator.AddAsync(validContact);
            await context.SaveChangesAsync();

            var cc = new CompanyCollaboratorDb
            {
                CollaboratorId = validContact.Id,
                CompanyId = validEvent.Id,
            };

            // Act
            var sqlMandateRepository = new SqlMandateRepository(_options);
            await sqlMandateRepository.CreateRoleAsync(cc);

            // Assert
            context.CompanyCollaborator.Count().Should().Be(1);
            context.CompanyCollaborator.First().CollaboratorId.Should().Be(validContact.Id);
            context.CompanyCollaborator.First().CompanyId.Should().Be(validEvent.Id);
        }

        [Fact]
        public async Task CreateRoleByEventAsync_ShouldThrowsNullArgument_WhenEventIsInvalid()
        {
            // Arrange
            var validEvent = EntityDbFactory.CompanyDb;
            var validContact = EntityDbFactory.CollaboratorDb;

            using var context = new MandateContext(_options);

            await context.Company.AddAsync(validEvent);
            await context.Collaborator.AddAsync(validContact);

            CompanyCollaboratorDb cc = null!;

            // Act
            var sqlMandateRepository = new SqlMandateRepository(_options);
            Func<Task> act = async () => await sqlMandateRepository.CreateRoleAsync(cc);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task DeleteRoleByEventAsync_ShouldDeleteRole_WhenEventIsValid()
        {
            // Arrange
            var validEvent = EntityDbFactory.CompanyDb;
            var validContact = EntityDbFactory.CollaboratorDb;

            using var context = new MandateContext(_options);

            await context.Company.AddAsync(validEvent);
            await context.Collaborator.AddAsync(validContact);
            await context.SaveChangesAsync();

            var cc = new CompanyCollaboratorDb
            {
                CollaboratorId = validContact.Id,
                CompanyId = validEvent.Id,
            };
            var sqlMandateRepository = new SqlMandateRepository(_options);
            await sqlMandateRepository.CreateRoleAsync(cc);

            // Act
            await sqlMandateRepository.DeleteRoleAsync(cc);

            // Assert
            context.CompanyCollaborator.Count().Should().Be(0);
        }
    }
}
