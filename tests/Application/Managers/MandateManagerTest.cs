// <copyright file="MandateManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.Managers
{
    public class MandateManagerTest
    {
        [Fact]
        public async Task CreateMandate()
        {
            var signatory = TestHelper.GetSignatory();
            var adress = TestHelper.GetAddress();
            Bban bban = TestHelper.GetBban();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"));

            var bank = TestHelper.GetBank("carteId", true);
            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            databaseService.Setup(r => r.GetBankByCodeAsync("code"))
                .ReturnsAsync(bank)
                .Verifiable();

            var companyManager = new Mock<ICompanyManager>(MockBehavior.Strict);
            companyManager.Setup(r => r.GetCompanyByErpId("1000332927"))
                .ReturnsAsync(company)
                .Verifiable();

            var createdCompany = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), "12345");
            var jeDeclareService = new Mock<IJeDeclareService>(MockBehavior.Strict);
            jeDeclareService.Setup(r => r.CreateFolderAsync(company))
                .ReturnsAsync(createdCompany)
                .Verifiable();

            databaseService.Setup(ds => ds.CreateFolderAsync("12345", new Guid("00000000-0000-0000-0000-000000000001")))
                .ReturnsAsync(It.Is<Company>(f => f != null))
                .Verifiable();

            databaseService.Setup(ds =>
                ds.CheckCollecteConfigExist(
                    It.Is<Bban>(b =>
                        b.BankCode == bank.Code &&
                        b.BranchCode == bban.BranchCode &&
                        b.AccountNumber == bban.AccountNumber &&
                        b.CheckDigits == bban.CheckDigits)))
                .ReturnsAsync(false)
                .Verifiable();

            var createdBban = TestHelper.GetBban("56789");
            jeDeclareService.Setup(js => js.AddRibToFolderAsync("12345", bban, signatory))
                .ReturnsAsync(createdBban)
                .Verifiable();

            var createdCollectionSQL = TestHelper.GetCollection(new Guid("00000000-0000-0000-0000-000000000001"));
            databaseService.Setup(ds => ds.CreateCollection("1000332927", new Guid("00000000-0000-0000-0000-000000000001"), bban))
                .ReturnsAsync(createdCollectionSQL)
               .Verifiable();

            var status = TestHelper.GetStatus();
            databaseService.Setup(ds =>
                ds.CreateStatus(
                    It.Is<Guid>(g => g == createdCollectionSQL.Id),
                    It.Is<Status>(s => s.StatusCode == status.StatusCode && s.StatusName == status.StatusName)))
                .ReturnsAsync(It.Is<Status>(s => s != null))
                .Verifiable();

            var createdCollectionJdc = TestHelper.GetCollection(new Guid("00000000-0000-0000-0000-000000000001"), "0987");
            jeDeclareService.Setup(js => js.CreateCollecteConfigurationAsync("12345", createdBban))
                .ReturnsAsync(createdCollectionJdc)
                .Verifiable();

            databaseService.Setup(ds => ds.UpdateCollection(createdCollectionSQL.Id, createdCollectionJdc))
                .ReturnsAsync(It.Is<Collection>(c => c != null))
                .Verifiable();

            databaseService.Setup(ds => ds.InsertServicesProviderIds(createdCollectionSQL.Id, createdCollectionJdc.CollectionServicesProviderId, createdBban.BbanServicesProviderId))
                .ReturnsAsync(It.Is<Collection>(jdc => jdc != null))
                .Verifiable();

            var statusCreated = TestHelper.GetStatus(CollectionStatus.InProgress, "Actif");

            databaseService.Setup(ds =>
                ds.CreateStatus(
                    It.Is<Guid>(g => g == createdCollectionSQL.Id),
                    It.Is<Status>(s => s.StatusCode == statusCreated.StatusCode && s.StatusName == statusCreated.StatusName)))
                .ReturnsAsync(It.Is<Status>(s => s != null))
                .Verifiable();

            var mandateManager = new MandateManager(databaseService.Object, companyManager.Object, jeDeclareService.Object);

            MandateCreation mandate = new MandateCreation("1000332927", signatory, adress, bban);

            var result = await mandateManager.CreateMandate(mandate).ConfigureAwait(false);
            result.Should().Be(createdCollectionSQL.Id);

            jeDeclareService.VerifyAll();
            databaseService.VerifyAll();
            companyManager.VerifyAll();
        }

        [Fact]
        public async Task CreateMandate_Partner_Bank_Exception()
        {
            var signatory = TestHelper.GetSignatory();
            var adress = TestHelper.GetAddress();
            Bban bban = TestHelper.GetBban();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"));

            var bank = TestHelper.GetBank();
            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            databaseService.Setup(r => r.GetBankByCodeAsync("code"))
                .ReturnsAsync(bank)
                .Verifiable();

            var companyManager = new Mock<ICompanyManager>(MockBehavior.Strict);
            companyManager.Setup(r => r.GetCompanyByErpId("1000332927"))
                .ReturnsAsync(company)
                .Verifiable();

            var createdCompany = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), "12345");
            var jeDeclareService = new Mock<IJeDeclareService>(MockBehavior.Strict);
            jeDeclareService.Setup(r => r.CreateFolderAsync(company))
                .ReturnsAsync(createdCompany)
                .Verifiable();

            databaseService.Setup(ds => ds.CreateFolderAsync("12345", new Guid("00000000-0000-0000-0000-000000000001")))
                .ReturnsAsync(It.Is<Company>(f => f != null))
                .Verifiable();

            var mandateManager = new MandateManager(databaseService.Object, companyManager.Object, jeDeclareService.Object);

            MandateCreation mandate = new MandateCreation("1000332927", signatory, adress, bban);

            Func<Task> acttion = () => mandateManager.CreateMandate(mandate);
            await acttion.Should().ThrowExactlyAsync<ApplicationException>()
                .WithMessage("L'établissement bancaire code n'est pas partenaire de JeDeclare.com mais est défini sans connexion à une carte EBICs.");

            jeDeclareService.VerifyAll();
            databaseService.VerifyAll();
            companyManager.VerifyAll();
        }

        [Fact]
        public async Task CreateMandate_Collecte_Config_Exist_Exception()
        {
            var signatory = TestHelper.GetSignatory();
            var adress = TestHelper.GetAddress();
            Bban bban = TestHelper.GetBban();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"));

            var bank = TestHelper.GetBank("carteId", true);
            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            databaseService.Setup(r => r.GetBankByCodeAsync("code"))
                .ReturnsAsync(bank)
                .Verifiable();

            var companyManager = new Mock<ICompanyManager>(MockBehavior.Strict);
            companyManager.Setup(r => r.GetCompanyByErpId("1000332927"))
                .ReturnsAsync(company)
                .Verifiable();

            var createdCompany = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), "12345");
            var jeDeclareService = new Mock<IJeDeclareService>(MockBehavior.Strict);
            jeDeclareService.Setup(r => r.CreateFolderAsync(company))
                .ReturnsAsync(createdCompany)
                .Verifiable();

            databaseService.Setup(ds => ds.CreateFolderAsync("12345", new Guid("00000000-0000-0000-0000-000000000001")))
                .ReturnsAsync(It.Is<Company>(f => f != null))
                .Verifiable();

            databaseService.Setup(ds =>
                ds.CheckCollecteConfigExist(
                    It.Is<Bban>(b =>
                        b.BankCode == bank.Code &&
                        b.BranchCode == bban.BranchCode &&
                        b.AccountNumber == bban.AccountNumber &&
                        b.CheckDigits == bban.CheckDigits)))
                .ReturnsAsync(true)
                .Verifiable();

            var mandateManager = new MandateManager(databaseService.Object, companyManager.Object, jeDeclareService.Object);

            MandateCreation mandate = new MandateCreation("1000332927", signatory, adress, bban);

            Func<Task> acttion = () => mandateManager.CreateMandate(mandate);
            await acttion.Should().ThrowExactlyAsync<ApplicationException>()
                .WithMessage("Il existe une configuration de collecte pour ce RIB code-02408-00011269900-58.");

            jeDeclareService.VerifyAll();
            databaseService.VerifyAll();
            companyManager.VerifyAll();
        }

        [Fact]
        public async Task GetAllCollectionsAsync_Case_OK()
        {
            var query = new CollectionQueryDto(
                 "search",
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new List<int>() { -1, 3 },
                 10,
                 0,
                 SortOrder.Ascending,
                 CollectionSortCriteria.AccountNumber,
                 "collab@email.com");

            Collaborator collaborator = EntityFactory.Collaborator;

            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            databaseService.Setup(r => r.GetCollaboratorByEmail("collab@email.com"))
                .ReturnsAsync(collaborator)
                .Verifiable();

            PagedMandate pagedMandate = EntityFactory.Page(new List<Collection> { EntityFactory.Collection });
            databaseService.Setup(r => r.GetAllCollectionsAsync(query, new Guid("00000001-0000-0000-0000-000000000000")))
                .ReturnsAsync(pagedMandate)
                .Verifiable();

            var mandateManager = new MandateManager(databaseService.Object, new Mock<ICompanyManager>(MockBehavior.Strict).Object, new Mock<IJeDeclareService>(MockBehavior.Strict).Object);

            var result = await mandateManager.GetAllCollectionsAsync(query).ConfigureAwait(false);
            result.Should().BeEquivalentTo(pagedMandate);

            databaseService.VerifyAll();
        }

        [Fact]
        public async Task GetAllCollectionsAsync_When_GetCollaboratorByEmail_Throw_Exception()
        {
            var query = new CollectionQueryDto(
                 "search",
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new List<int>() { -1, 3 },
                 10,
                 0,
                 SortOrder.Ascending,
                 CollectionSortCriteria.AccountNumber,
                 "collab@email.com");

            // Arrange
            var databaseService = new Mock<IDatabaseService>();
            databaseService.Setup(x => x.GetCollaboratorByEmail("collab@email.com"))
                               .ThrowsAsync(new Exception("message"));

            var mandateManager = new MandateManager(databaseService.Object, new Mock<ICompanyManager>(MockBehavior.Strict).Object, new Mock<IJeDeclareService>(MockBehavior.Strict).Object);

            Func<Task> action = async () => await mandateManager.GetAllCollectionsAsync(query);
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            databaseService.VerifyAll();
        }

        [Fact]
        public async Task GetAllCollectionsAsync_Throw_Exception()
        {
            var query = new CollectionQueryDto(
                 "search",
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new List<int>() { -1, 3 },
                 10,
                 0,
                 SortOrder.Ascending,
                 CollectionSortCriteria.AccountNumber,
                 "collab@email.com");

            Collaborator collaborator = new Collaborator(new Guid("00000001-0000-0000-0000-000000000000"), "collab@email.com", "fname", "lname");

            var databaseService = new Mock<IDatabaseService>();
            databaseService.Setup(x => x.GetCollaboratorByEmail("collab@email.com"))
                               .ReturnsAsync(collaborator);

            databaseService.Setup(x => x.GetAllCollectionsAsync(query, new Guid("00000001-0000-0000-0000-000000000000")))
                   .ThrowsAsync(new Exception("message"));

            var mandateManager = new MandateManager(databaseService.Object, new Mock<ICompanyManager>(MockBehavior.Strict).Object, new Mock<IJeDeclareService>(MockBehavior.Strict).Object);

            Func<Task> action = async () => await mandateManager.GetAllCollectionsAsync(query);
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            databaseService.VerifyAll();
        }
    }
}