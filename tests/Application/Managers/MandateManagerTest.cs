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
            companyManager.Setup(r => r.GetCompanyByErpIdAsync("1000332927"))
                .ReturnsAsync(company)
                .Verifiable();

            var createdCollectionSQL = TestHelper.GetCollection(new Guid("00000000-0000-0000-0000-000000000001"));
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

            databaseService.Setup(ds => ds.SaveSignatoryAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<Signatory>(), It.IsAny<Address>()))
                .Callback<Guid?, Guid?, Signatory, Address>((comp, coll, sign, adres) =>
                {
                    comp.Should().Be(company.Id);
                    coll.Should().Be(createdCollectionSQL.Id);
                    sign.Should().BeEquivalentTo(company.Signatory);
                    adres.Should().BeEquivalentTo(company.Address);
                })
                .Returns(Task.CompletedTask)
                .Verifiable();

            var createdBban = TestHelper.GetBban("56789");
            jeDeclareService.Setup(js => js.AddRibToFolderAsync("12345", bban, signatory))
                .ReturnsAsync(createdBban)
                .Verifiable();

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
            companyManager.Setup(r => r.GetCompanyByErpIdAsync("1000332927"))
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
            companyManager.Setup(r => r.GetCompanyByErpIdAsync("1000332927"))
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
                  string.Empty,
                  null,
                  null,
                  null,
                  null,
                  null,
                  10,
                  0,
                  SortOrder.Ascending,
                  CollectionSortCriteria.Name,
                  Guid.Empty);

            Company company = new Company(
                new Guid("00000001-0000-0000-0000-000000000000"),
                "cn",
                "12345678910",
                "123456789",
                string.Empty,
                null,
                null);

            Bank bank = new Bank("12345", "bn", "bg", string.Empty, new BankAgreement(JdcPartnership.NonPartner));

            Bban bban = new Bban("12345", "54321", "12345678901", "55", string.Empty, bank);

            Collection collection = new Collection(
                    new Guid("00000002-0000-0000-0000-000000000000"),
                    string.Empty,
                    company,
                    bban,
                    new DateTime(2022, 1, 1),
                    new DateTime(2022, 1, 1),
                    new Status(CollectionStatus.InProgress, "En cours"));

            var counters = new Counters(1, 1, 0, 0, 0, 0);
            var pm = new PagedMandate(counters, new List<Collection> { collection });

            var database = new Mock<IDatabaseService>(MockBehavior.Strict);
            database.Setup(i => i.GetAllCollectionsAsync(query))
                .ReturnsAsync(pm)
                .Verifiable();

            MandateManager manager = new MandateManager(
                database.Object,
                new Mock<ICompanyManager>(MockBehavior.Strict).Object,
                new Mock<IJeDeclareService>(MockBehavior.Strict).Object);

            var result = await manager.GetAllCollectionsAsync(query);

            result.Should().BeEquivalentTo(pm);

            database.VerifyAll();
        }

        [Fact]
        public async Task GetAllCollectionsAsync_When_Service_Throw_Exception()
        {
            var query = new CollectionQueryDto(
                  string.Empty,
                  null,
                  null,
                  null,
                  null,
                  null,
                  10,
                  0,
                  SortOrder.Ascending,
                  CollectionSortCriteria.Name,
                  Guid.Empty);

            var database = new Mock<IDatabaseService>(MockBehavior.Strict);
            database.Setup(i => i.GetAllCollectionsAsync(query))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            MandateManager manager = new MandateManager(
                database.Object,
                new Mock<ICompanyManager>(MockBehavior.Strict).Object,
                new Mock<IJeDeclareService>(MockBehavior.Strict).Object);

            Func<Task> action = async () => await manager.GetAllCollectionsAsync(query);
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            database.VerifyAll();
        }
    }
}