// <copyright file="ClientExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    public class ClientExtensionsTest
    {
        [Fact]
        public void AddressToModel()
        {
            var entity = new Client.Address("a", "b", "c", "d", "e");
            var result = entity.ToModel();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Address("a", "b", "c", "d", "e"));
        }

        [Fact]
        public void BbanToModel()
        {
            var entity = new Client.Bban("a", "b", "c", "d");
            var result = entity.ToModel();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Bban("a", "b", "c", "d", null, null));
        }

        [Fact]
        public void CollectionCreationCommandToModel()
        {
            var address = new Client.Address("a", "b", "c", "d", "e");
            var bban = new Client.Bban("a", "b", "c", "d");
            var signatory = new Client.Signatory("a", "b", "c", "d");
            var entity = new Client.CollectionCreationCommand("a", signatory, address, bban);
            var result = entity.ToModel();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new CollectionCreationCommand("a", signatory.ToModel(), address.ToModel(), bban.ToModel()));
        }

        [Fact]
        public void SignatoryToModel()
        {
            var entity = new Client.Signatory("a", "b", "c", "d");
            var result = entity.ToModel();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Signatory("a", "b", "c", "d"));
        }

        [Fact]
        public void ToBankDetail()
        {
            var entity = new Bank("a", "b", "c", "d", new BankAgreement(JdcPartnership.NonPartner));
            var result = entity.ToBankDetail();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Client.BankDetail("a", "b", new Client.BankJdcDetail("NonPartner")));
        }

        [Fact]
        public void ToCollectionSummary()
        {
            Guid id = Guid.NewGuid();
            Company company = new Company(101, "mega", "45207964300014", "1999156874", string.Empty, null, null);
            Bank? bank = new Bank("12345", "biap", "biap group", string.Empty, new BankAgreement(JdcPartnership.NonPartner));
            Bban bban = new Bban("12345", "56789", "12345678901", "88", "6789", bank);
            Status status = new Status(CollectionStatus.ToDo, "An example of Jdctatus Description", Mandate.JdcCollectionStatus.Creation_InProgress, null);

            Collection collection = new Collection(
                id,
                "12346",
                company,
                bban,
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                status, null, destinationTool: "testdestination");

            var model = collection.ToCollectionSummary();

            var collectionBankInfo = new Client.CollectionBankInfo(
                bankName: "biap",
                accountNumber: "12345678901",
                jdcPartnership: 2);

            var statusSummary = new Client.StatusInfo(
                statusCode: (int)CollectionStatus.ToDo,
                jdcStatusDescription: "An example of Jdctatus Description",
                jdcStatusCode: (int)Mandate.JdcCollectionStatus.Creation_InProgress);

            var expected = new Client.CollectionSummary(
                id: id,
                erpId: "1999156874",
                companyName: "mega",
                collectionBankInfo: collectionBankInfo,
                creationDate: new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                modificationDate: new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                statusInfo: statusSummary,
                ["CAN_DOWNLOAD_PREFILLED_MANDATE", "CAN_UPLOAD_SIGNED_MANDATE"], destinationTool: "testdestination");

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToCollectionSummaryForActiveStatus()
        {
            Guid id = Guid.NewGuid();
            Company company = new Company(101, "mega", "45207964300014", "1999156874", string.Empty, null, null);
            Bank? bank = new Bank("12345", "biap", "biap group", string.Empty, new BankAgreement(JdcPartnership.NonPartner));
            Bban bban = new Bban("12345", "56789", "12345678901", "88", "6789", bank);
            Status status = new Status(CollectionStatus.Active, "An example of Jdctatus Description", Mandate.JdcCollectionStatus.Creation_InProgress, null);

            Collection collection = new Collection(
                id,
                "12346",
                company,
                bban,
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                status, null, destinationTool: "testdestination");

            var model = collection.ToCollectionSummary();

            var collectionBankInfo = new Client.CollectionBankInfo(
                bankName: "biap",
                accountNumber: "12345678901",
                jdcPartnership: 2);

            var statusSummary = new Client.StatusInfo(
                statusCode: (int)CollectionStatus.Active,
                jdcStatusDescription: "An example of Jdctatus Description",
                jdcStatusCode: (int)Mandate.JdcCollectionStatus.Creation_InProgress);

            var expected = new Client.CollectionSummary(
                id: id,
                erpId: "1999156874",
                companyName: "mega",
                collectionBankInfo: collectionBankInfo,
                creationDate: new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                modificationDate: new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                statusInfo: statusSummary,
                ["CAN_DOWNLOAD_PREFILLED_MANDATE", "CAN_UPLOAD_SIGNED_MANDATE", "CAN_DOWNLOAD_SIGNED_MANDATE", "CAN_TERMINATE_TELECOLLECT"], destinationTool: "testdestination");

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToTechnicalCollectionSummary()
        {
            Guid id = Guid.NewGuid();
            Company company = new Company(101, "mega", "45207964300014", "1999156874", "12345", null, null);
            Bank? bank = new Bank("12345", "biap", "biap group", string.Empty, null!);
            Bban bban = new Bban("12345", "56789", "12345678901", "88", "6789", bank);
            Status status = new Status(CollectionStatus.ToDo, "todo", Mandate.JdcCollectionStatus.Creation_InProgress, null);

            Collection collection = new Collection(
                id,
                "12346",
                company,
                bban,
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                status,
                null);

            var model = collection.ToTechnicalCollectionSummary();

            var expected = new Client.TechnicalCollectionSummary(
                id,
                "12345",
                "6789",
                new Client.BankDetails(
                    "12345",
                    "56789",
                    "12345678901",
                    "88"),
                (int)CollectionStatus.ToDo);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToTechnicalCollectionSummary_WithNullAttributes()
        {
            Guid id = Guid.NewGuid();
            Company company = new Company(101, "mega", "45207964300014", "1999156874", "12345", null, null);
            Bank? bank = new Bank("12345", "biap", "biap group", string.Empty, null!);
            Status status = new Status(CollectionStatus.ToDo, "todo", Mandate.JdcCollectionStatus.Creation_InProgress, null);

            Collection collection = new Collection(
                id,
                "12346",
                company,
                null,
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                status, null);

            var model = collection.ToTechnicalCollectionSummary();

            var expected = new Client.TechnicalCollectionSummary(
                id,
                "12345",
                null!,
                new Client.BankDetails(
                    null!,
                    null!,
                    null!,
                    null!),
                (int)CollectionStatus.ToDo);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToModelTechnicalCollection()
        {
            Guid id = Guid.NewGuid();
            var technicalCollectionSummary = new Client.TechnicalCollectionSummary(
                id,
                "12345",
                "6789",
                new Client.BankDetails(
                    "12345",
                    "56789",
                    "12345678901",
                    "88"),
                (int)CollectionStatus.ToDo);

            var model = technicalCollectionSummary.ToModel();

            var expected = new TechnicalCollection(
                id,
                "12345",
                "6789",
                new BankDetails(
                    "12345",
                    "56789",
                    "12345678901",
                    "88"),
                "20");

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToModel()
        {
            var entity = new Client.CollectionQuery(
                "search",
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                new List<int>() { -1, 3 },
                10,
                0,
                "Ascending",
                "AccountNumber",
                "collab@email.com");

            var model = entity.ToModel();

            model.Should().BeEquivalentTo(new CollectionQueryDto(
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
                 "collab@email.com"));
        }

        [Fact]
        public void ToModel_WhenNoSortOrder_ShouldThrowException()
        {
            var entity = new Client.CollectionQuery(
                "search",
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                new List<int>() { -1, 3 },
                10,
                0,
                string.Empty,
                "AccountNumber",
                "collab@email.com");

            Action act = () => entity.ToModel();
            act.Should().Throw<InvalidCastException>().WithMessage("Invalid SortOrder. Allowed values are [ Ascending, Descending]");
        }

        [Fact]
        public void ToModel_WhenNoSortCreteria_ShouldThrowException()
        {
            var entity = new Client.CollectionQuery(
                "search",
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                new List<int>() { -1, 3 },
                10,
                0,
                "Ascending",
                string.Empty,
                "collab@email.com");

            Action act = () => entity.ToModel();
            act.Should().Throw<InvalidCastException>().WithMessage("Invalid SortCriteria. Allowed values are [ ErpId, Name, AccountNumber, BankName, CreationDate, ModificationDate, Status]");
        }

        [Fact]
        public void ToCountersDetail()
        {
            Counters counters = new Counters(10, 2, 3, 1, 2, 2);

            var model = counters.ToCountersDetail();

            model.Should().BeEquivalentTo(new Client.Counters(10, 2, 3, 1, 2, 2));
        }

        [Fact]
        public void ToPageMandateDetails()
        {
            Counters counters = new Counters(1, 1, 0, 0, 0, 0);
            Company company = new Company(
                101,
                "cn",
                "12345678910",
                "123456789",
                string.Empty,
                null,
                null);

            Bank? bank = new Bank("12345", "biap", "biap group", string.Empty, new BankAgreement(JdcPartnership.NonPartner));
            Bban bban = new Bban("12345", "54321", "12345678901", "55", string.Empty, bank);

            Collection collection = new Collection(
                    new Guid("00000002-0000-0000-0000-000000000000"),
                    string.Empty,
                    company,
                    bban,
                    new DateTime(2022, 1, 1),
                    new DateTime(2022, 1, 1),
                    new Status(default, string.Empty, default, null),
                    null);

            List<Collection> collections = new List<Collection>()
            {
                collection,
            };

            PagedMandate page1 = new PagedMandate(counters, collections);

            var model = page1.ToPageMandateDetails();
            var expectedCounters = new Client.Counters(1, 1, 0, 0, 0, 0);
            var expectedCollections = new List<Client.CollectionSummary>()
            {
               collection.ToCollectionSummary(),
            };

            model.Should().BeEquivalentTo(new Client.PagedMandate(expectedCounters, expectedCollections));
        }

        [Fact]
        public void ToPageTechnicalMandateDetails()
        {
            Counters counters = new Counters(1, 1, 0, 0, 0, 0);
            Company company = new Company(
                101,
                "cn",
                "12345678910",
                "123456789",
                string.Empty,
                null,
                null);

            Bban bban = new Bban("12345", "54321", "12345678901", "55", string.Empty, null);

            Collection collection = new Collection(
                    new Guid("00000002-0000-0000-0000-000000000000"),
                    string.Empty,
                    company,
                    bban,
                    new DateTime(2022, 1, 1),
                    new DateTime(2022, 1, 1),
                    new Status(default, string.Empty, default, null),
                    null);

            List<Collection> collections = new List<Collection>()
            {
                collection,
            };

            PagedTechnicalMandate page1 = new PagedTechnicalMandate(counters, collections);

            var model = page1.ToPageTechnicalMandateDetails();
            var expectedCounters = new Client.Counters(1, 1, 0, 0, 0, 0);
            var expectedCollections = new List<Client.TechnicalCollectionSummary>()
            {
               collection.ToTechnicalCollectionSummary(),
            };

            model.Should().BeEquivalentTo(new Client.PagedTechnicalMandate(expectedCollections));
        }

        [Fact]
        public void ToRibString()
        {
            Bban bban = new Bban("12345", "54321", "12345678901", "55", string.Empty, null);
            string res = bban.ToRibString();
            res.Should().Be("12345-54321-12345678901-55");
        }

        [Fact]
        public void ToRibString_When_BbanNull()
        {
            Bban bban = null!;
            string res = bban.ToRibString();
            res.Should().Be(string.Empty);
        }
    }
}
