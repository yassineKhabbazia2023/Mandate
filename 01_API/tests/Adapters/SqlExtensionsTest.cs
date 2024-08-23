// <copyright file="SqlExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    public class SqlExtensionsTest
    {
        [Fact]
        public void BankToModel()
        {
            var entity = new Sql.RefBankDb() { BankCode = "c", BankName = "n", BankGroup = "g", EbicsCardId = "e", JdcPartnership = (Sql.JdcPartnership)2 };
            var result = entity.ToModel();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Bank("c", "n", "g", "e", new BankAgreement(JdcPartnership.NonPartner)));
        }

        [Fact]
        public void ToSqlTest()
        {
            CollectionQueryDto queryDto = new CollectionQueryDto(
                creationDateEnd: new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                creationDateStart: new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                limit: 10,
                modificationDateStart: new DateTime(2023, 10, 3, 0, 0, 0, DateTimeKind.Utc),
                modificationDateEnd: new DateTime(2023, 10, 4, 0, 0, 0, DateTimeKind.Utc),
                searchTerm: "companyName",
                skip: 0,
                sortCriteria: CollectionSortCriteria.ModificationDate,
                sortOrder: SortOrder.Ascending,
                statusCodes: new List<int> { 1, 2 },
                collaboratorEmail: "collab@email.com");

            var res = queryDto.ToSql(101);

            res.Should().BeEquivalentTo(new Sql.CollectionQuery
            {
                CreationDateEnd = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                CreationDateStart = new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                Limit = 10,
                ModificationDateStart = new DateTime(2023, 10, 3, 0, 0, 0, DateTimeKind.Utc),
                ModificationDateEnd = new DateTime(2023, 10, 4, 0, 0, 0, DateTimeKind.Utc),
                SearchTerm = "companyName",
                Skip = 0,
                SortCriteria = Sql.CollectionSortCriteria.ModificationDate,
                SortOrder = Sql.SortOrder.Ascending,
                StatusCodes = new List<int> { 1, 2 },
                CollaboratorId = 101,
            });
        }

        [Fact]
        public void ToModel_ShouldConvertCompanyDbToCompanyModelCorrectly()
        {
            // Arrange
            var companyDb = new Sql.CompanyDb
            {
                Id = 101,
                Name = "Test Company",
                SiretNumber = "123456789",
                ErpId = "ERP123",
                JeDeclareFolder = new Sql.JeDeclareFolderDb { JdcDossierId = "JDC123" },
                Personal = new Sql.PersonalDb
                {
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
            };

            // Act
            var result = companyDb.ToModel();

            // Assert
            result.Id.Should().Be(companyDb.Id);
            result.Name.Should().Be("Test Company");
            result.SiretNumber.Should().Be("123456789");
            result.ErpId.Should().Be("ERP123");
            result.BankServicesProviderId.Should().Be("JDC123");
            result?.Signatory?.Title.Should().Be("Mr.");
            result?.Signatory?.FirstName.Should().Be("John");
            result?.Signatory?.LastName.Should().Be("Doe");
            result?.Signatory?.Email.Should().Be("john.doe@example.com");
            result?.Address?.Street.Should().Be("123 Main St");
            result?.Address?.ZipCode.Should().Be("12345");
            result?.Address?.City.Should().Be("Sample City");
            result?.Address?.Country.Should().Be("ExampleLand");
            result?.Address?.Complements.Should().Be("Apt 4B");
        }

        [Fact]
        public void ToModel_WithNullProperties_ShouldHandleNullsGracefully()
        {
            // Arrange
            var companyDb = new Sql.CompanyDb
            {
                Id = 101,
                Name = "Test Company",
                SiretNumber = "123456789",
                ErpId = "ERP123",
                JeDeclareFolder = null,
                Personal = null,
            };

            // Act
            var result = companyDb.ToModel();

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(companyDb.Id);
            result.Name.Should().Be("Test Company");
            result.SiretNumber.Should().Be("123456789");
            result.ErpId.Should().Be("ERP123");
            result.BankServicesProviderId.Should().BeNull();
            result.Signatory!.Email.Should().BeNull();
            result.Signatory.FirstName.Should().BeNull();
            result.Signatory.LastName.Should().BeNull();
            result.Signatory.Title.Should().BeNull();
            result.Address!.Street.Should().BeNull();
            result.Address.Complements.Should().BeNull();
            result.Address.ZipCode.Should().BeNull();
            result.Address.City.Should().BeNull();
            result.Address.Country.Should().BeNull();
        }

        [Fact]
        public void ToModel()
        {
            Bban entity = EntityFactory.Bban;

            var res = entity.ToSql(101);

            var expected = new Sql.CollectionDb()
            {
                BankCode = "12345",
                BranchCode = "54321",
                AccountNumber = "12345678901",
                CheckDigits = "01",
                CompanyId = 101,
            };
            res.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToPersonalCollection()
        {
            var entity = new CollectionCreationCommand(
                "12345",
                EntityFactory.Signatory,
                EntityFactory.Address,
                EntityFactory.Bban);

            var result = entity.ToPersonalCollection();

            var expected = new Sql.PersonalDb
            {
                Title = "Mme",
                FirstName = "First",
                LastName = "Last",
                Email = "first.last@outlook.com",
                City = "Paris",
                ZipCode = "75001",
                Street = "street",
                Complements = "complements",
                Country = "France",
            };

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void DefaultStatus()
        {
            Sql.StatusDb status = SqlExtensions.DefaultStatus();

            status.IsCurrent.Should().BeTrue();
            status.StatusCode.Should().Be((int)CollectionStatus.Creation_Inprogress);
            status.StatusDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void InitialCreateStatus()
        {
            Sql.StatusDb status = SqlExtensions.InitialCreateStatus();

            status.IsCurrent.Should().BeFalse();
            status.StatusCode.Should().Be((int)JdcCollectionStatus.InitialCreate);
            status.StatusDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Status_ToModel()
        {
            var entity = new Sql.StatusDb()
            {
                RefStatusCode = new Sql.RefStatusCodeDb()
                {
                    PulseCode = 30,
                    StatusCode = 1,
                    StatusNameFr = "statusName",
                },
            };

            var result = entity.ToModel();

            result.Should().BeEquivalentTo(new Status(CollectionStatus.InProgress, "statusName"));
        }

        [Fact]
        public void ToSignatory()
        {
            var entity = new Sql.PersonalDb()
            {
                Id = Guid.NewGuid(),
                City = "city",
                Complements = "comp",
                ZipCode = "43598",
                Country = "country",
                Street = "street",
                FirstName = "first",
                LastName = "last",
                Title = "M",
                Email = "email@email.com",
                CompanyId = 101,
                CollectionId = Guid.NewGuid(),
            };

            var result = entity.ToSignatory();

            var expected = new Signatory("M", "first", "last", "email@email.com");

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToAdress()
        {
            var entity = new Sql.PersonalDb()
            {
                Id = Guid.NewGuid(),
                City = "city",
                Complements = "comp",
                ZipCode = "43598",
                Country = "country",
                Street = "street",
                FirstName = "first",
                LastName = "last",
                Title = "M",
                Email = "email@email.com",
                CompanyId = 101,
                CollectionId = Guid.NewGuid(),
            };

            var result = entity.ToAdress();

            var expected = new Address("street", "comp", "43598", "city", "country");

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToBbanModel()
        {
            var entity = new Sql.CollectionDb()
            {
                Id = Guid.NewGuid(),
                BankCode = "30003",
                BranchCode = "12345",
                AccountNumber = "12345678901",
                CheckDigits = "11",
                JeDeclareCollection = new Sql.JeDeclareCollectionDb()
                {
                    JdcReleveId = "6789",
                    JdcRibId = "6781",
                },
                Bank = new Sql.RefBankDb()
                {
                    BankCode = "c",
                    BankName = "n",
                    BankGroup = "g",
                    EbicsCardId = "e",
                    JdcPartnership = (Sql.JdcPartnership)2,
                },
            };

            var result = entity.ToBbanModel();

            var expected = new Bban(
                "30003",
                "12345",
                "12345678901",
                "11",
                "6781",
                new Bank("c", "n", "g", "e", new BankAgreement(JdcPartnership.NonPartner)));

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToModelCollection_ThrowApplicationException_WhenNoCurrentStatus()
        {
            var entity = new Sql.CollectionDb()
            {
                Id = Guid.NewGuid(),
                BankCode = "30003",
                BranchCode = "12345",
                AccountNumber = "12345678901",
                CheckDigits = "11",
                JeDeclareCollection = new Sql.JeDeclareCollectionDb()
                {
                    JdcReleveId = "6789",
                    JdcRibId = "6781",
                },
                Bank = new Sql.RefBankDb()
                {
                    BankCode = "c",
                    BankName = "n",
                    BankGroup = "g",
                    EbicsCardId = "e",
                    JdcPartnership = (Sql.JdcPartnership)2,
                },
                Statuses = new List<Sql.StatusDb>
                {
                    new Sql.StatusDb()
                    {
                        Id = Guid.NewGuid(),
                        IsCurrent = false,
                        StatusDate = DateTime.Now,
                        StatusCode = -1,
                    },
                },
            };

            var act = () => entity.ToModel();

            act.Should().Throw<ApplicationException>().WithMessage("SqlExtensions - ToModel : Error while parsing Collection currentStatus is null.");
        }

        [Fact]
        public void ToModelCollection_ThrowApplicationException_WhenNoCreationStatus()
        {
            var entity = new Sql.CollectionDb()
            {
                Id = Guid.NewGuid(),
                BankCode = "30003",
                BranchCode = "12345",
                AccountNumber = "12345678901",
                CheckDigits = "11",
                JeDeclareCollection = new Sql.JeDeclareCollectionDb()
                {
                    JdcReleveId = "6789",
                    JdcRibId = "6781",
                },
                Bank = new Sql.RefBankDb()
                {
                    BankCode = "c",
                    BankName = "n",
                    BankGroup = "g",
                    EbicsCardId = "e",
                    JdcPartnership = (Sql.JdcPartnership)2,
                },
                Statuses = new List<Sql.StatusDb>
                {
                    new Sql.StatusDb()
                    {
                        Id = Guid.NewGuid(),
                        IsCurrent = true,
                        StatusDate = DateTime.Now,
                        StatusCode = 20,
                    },
                },
            };

            var act = () => entity.ToModel();

            act.Should().Throw<ApplicationException>().WithMessage("SqlExtensions - ToModel : Error while parsing Collection creationStatus is null.");
        }

        [Fact]
        public void ToStatusesDB()
        {
            // Arrange
            Bban bban1 = new Bban("13507", "00014", "31464482121", "77", "8909441", null);
            var address = new Address("12 RUE DES 2 NATIONS", string.Empty, "59250", "HALLUIN", "France");
            var signatory = new Signatory("m", "OLIVIER", "BRUNELAT", "toto@gmail.com");
            var company = new Company(default, "SPORT FIT SAS", "83455379400019", "1000326214", "19820673", signatory, address);
            Status status = new Status(CollectionStatus.ToDo, "En cours");
            var collection = new Collection(Guid.Empty, "8909440", company, bban1, new DateTime(2019, 10, 10, 8, 54, 3), new DateTime(2019, 10, 10, 8, 54, 3), status);

            var expected = new List<Sql.StatusDb>()
            {
                new()
                    {
                        Id = Guid.NewGuid(),
                        CollectionId = collection.Id,
                        StatusCode = -1,
                        IsCurrent = true,
                        StatusDate = DateTime.UtcNow,
                    },
            };

            // Act
            var result = collection.ToStatusesDB();

            // Assert
            result.Should().HaveCount(1);

            result.First().CollectionId.Should().Be(collection.Id);
            result.First().CollectionStatusCode.Should().Be(expected.First().CollectionStatusCode);
            result.First().CreatedBy.Should().Be(expected.First().CreatedBy);
            result.First().IsCurrent.Should().BeTrue();
            result.First().MandateFile.Should().BeEquivalentTo(expected.First().MandateFile);
            result.First().RefStatusCode.Should().Be(expected.First().RefStatusCode);
            result.First().StatusCode.Should().Be(-1);
        }

        [Fact]
        public void ToPersonalDb()
        {
            // Arrange
            Bban bban1 = new Bban("13507", "00014", "31464482121", "77", "8909441", null);
            var address = new Address("12 RUE DES 2 NATIONS", string.Empty, "59250", "HALLUIN", "France");
            var signatory = new Signatory("m", "OLIVIER", "BRUNELAT", "toto@gmail.com");
            var company = new Company(default, "SPORT FIT SAS", "83455379400019", "1000326214", "19820673", signatory, address);
            Status status = new Status(CollectionStatus.ToDo, "En cours");
            var collection = new Collection(Guid.Empty, "8909440", company, bban1, new DateTime(2019, 10, 10, 8, 54, 3), new DateTime(2019, 10, 10, 8, 54, 3), status);

            var expected = new Sql.PersonalDb()
            {
                City = collection.Company?.Address?.City,
                Country = collection.Company?.Address?.Country,
                Email = collection.Company?.Signatory?.Email!,
                FirstName = collection.Company?.Signatory?.FirstName,
                LastName = collection.Company?.Signatory?.LastName,
                Street = collection.Company?.Address?.Street,
                ZipCode = collection.Company?.Address?.ZipCode,
                Complements = collection.Company?.Address?.Complements,
                Title = collection.Company?.Signatory?.Title,
            };

            // Act
            var result = collection.Company!.ToPersonalDb();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToDeclareCollectionDb()
        {
            // Arrange
            Bban bban1 = new Bban("13507", "00014", "31464482121", "77", "8909441", null);
            var address = new Address("12 RUE DES 2 NATIONS", string.Empty, "59250", "HALLUIN", "France");
            var signatory = new Signatory("m", "OLIVIER", "BRUNELAT", "toto@gmail.com");
            var company = new Company(default, "SPORT FIT SAS", "83455379400019", "1000326214", "19820673", signatory, address);
            Status status = new Status(CollectionStatus.ToDo, "En cours");
            var collection = new Collection(Guid.Empty, "8909440", company, bban1, new DateTime(2019, 10, 10, 8, 54, 3), new DateTime(2019, 10, 10, 8, 54, 3), status);

            var expected = new Sql.JeDeclareCollectionDb()
            {
                CollectionId = collection.Id,
                JdcReleveId = collection.CollectionServicesProviderId,
                JdcRibId = collection.Bban?.BbanServicesProviderId,
            };

            // Act
            var result = collection.ToDeclareCollectionDb();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToJeDeclareFolderDb()
        {
            // Arrange
            Bban bban1 = new Bban("13507", "00014", "31464482121", "77", "8909441", null);
            var address = new Address("12 RUE DES 2 NATIONS", string.Empty, "59250", "HALLUIN", "France");
            var signatory = new Signatory("m", "OLIVIER", "BRUNELAT", "toto@gmail.com");
            var company = new Company(default, "SPORT FIT SAS", "83455379400019", "1000326214", "19820673", signatory, address);
            Status status = new Status(CollectionStatus.ToDo, "En cours");
            var collection = new Collection(Guid.Empty, "8909440", company, bban1, new DateTime(2019, 10, 10, 8, 54, 3), new DateTime(2019, 10, 10, 8, 54, 3), status);

            var expected = new Sql.JeDeclareFolderDb()
            {
                CompanyId = company.Id,
                JdcDossierId = company.BankServicesProviderId,
            };

            // Act
            var result = company.ToJeDeclareFolderDb();

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToCompanyDB()
        {
            // Arrange
            Bban bban1 = new Bban("13507", "00014", "31464482121", "77", "8909441", null);
            var address = new Address("12 RUE DES 2 NATIONS", string.Empty, "59250", "HALLUIN", "France");
            var signatory = new Signatory("m", "OLIVIER", "BRUNELAT", "toto@gmail.com");
            var company = new Company(default, "SPORT FIT SAS", "83455379400019", "1000326214", "19820673", signatory, address);
            Status status = new Status(CollectionStatus.ToDo, "En cours");
            var collection = new Collection(Guid.Empty, "8909440", company, bban1, new DateTime(2019, 10, 10, 8, 54, 3), new DateTime(2019, 10, 10, 8, 54, 3), status);

            var expected = new Sql.CompanyDb()
            {
                Id = collection.Company!.Id,
                Name = collection.Company.Name,
                ErpId = collection.Company.ErpId,
                SiretNumber = collection.Company.SiretNumber,
                CompanyCollaborators = new List<Sql.CompanyCollaboratorDb>(),
                JeDeclareFolder = collection.Company.ToJeDeclareFolderDb(),
            };

            // Act
            var result = collection.Company.ToSql();

            // Assert
            result.Id.Should().Be(collection.Company.Id);
            result.Name.Should().Be(collection.Company.Name);
            result.ErpId.Should().Be(collection.Company.ErpId);
            result.SiretNumber.Should().Be(collection.Company.SiretNumber);
            result.CompanyCollaborators.Should().BeEquivalentTo(expected.CompanyCollaborators);
        }

        [Fact]
        public void ToCollectionDB()
        {
            // Arrange
            var bankAgrement = new BankAgreement(JdcPartnership.Partner);
            var bank = new Bank("code", "name", "group", "ebicsCardId", bankAgrement);
            Bban bban1 = new Bban("13507", "00014", "31464482121", "77", "8909441", bank);
            var address = new Address("12 RUE DES 2 NATIONS", string.Empty, "59250", "HALLUIN", "France");
            var signatory = new Signatory("m", "OLIVIER", "BRUNELAT", "toto@gmail.com");
            var company = new Company(default, "SPORT FIT SAS", "83455379400019", "1000326214", "19820673", signatory, address);
            Status status = new Status(CollectionStatus.ToDo, "En cours");
            var collection = new Collection(Guid.Empty, "8909440", company, bban1, new DateTime(2019, 10, 10, 8, 54, 3), new DateTime(2019, 10, 10, 8, 54, 3), status);

            var expected = new Sql.CollectionDb()
            {
                Id = collection.Id,
                AccountNumber = collection.Bban!.AccountNumber.ToString(),
                BranchCode = collection.Bban!.BranchCode,
                CheckDigits = collection.Bban!.CheckDigits,
                BankCode = collection.Bban!.BankCode,
                Company = company.ToSql(),
                JeDeclareCollection = collection.ToDeclareCollectionDb(),
                LinkType = null,
                Personal = collection.Company!.ToPersonalDb(),
                CompanyId = collection.Company!.Id,
                RejectReason = null,
                Statuses = collection.ToStatusesDB(),
            };

            // Act
            var result = collection.ToCollectionDB(101);

            // Assert
            result.Id.Should().Be(collection.Id);
            result.AccountNumber.Should().Be(expected.AccountNumber);
            result.BranchCode.Should().Be(expected.BranchCode);
            result.CheckDigits.Should().Be(expected.CheckDigits);
            result.BankCode.Should().Be(expected.BankCode);
            result.CompanyId.Should().Be(101);
            result.RejectReason.Should().Be(expected.RejectReason);
        }

        [Fact]
        public void ToMandateLogDB()
        {
            CustomException exception = new CustomException(
                ExceptionType.NoAccountNumberMatchDoubleSiret,
                "message",
                new Exception("innermessage"));

            Company company = new Company(
                1,
                "name",
                "12345678901234",
                "1234567890",
                "1234",
                It.IsAny<Signatory>(),
                It.IsAny<Address>());

            Bban bban = new Bban(
                "12345",
                "54321",
                "12345678910",
                "11",
                "4321",
                It.IsAny<Bank>());

            Collection collection = new Collection(
                It.IsAny<Guid>(),
                "4321",
                company,
                bban,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<Status>());

            var result = collection.ToMandateLogDB(exception);
            result.SiretNumber.Should().Be("12345678901234");
            result.ErpId.Should().Be("1234567890");
            result.BankCode.Should().Be("12345");
            result.AccountNumber.Should().Be("12345678910");
            result.BranchCode.Should().Be("54321");
            result.CheckDigits.Should().Be("11");
            result.JdcDossierId.Should().Be("1234");
            result.JdcReleveId.Should().Be("4321");
            result.JdcRibId.Should().Be("4321");
            result.ExceptionMessage.Should().Be("message");
            result.InnerExceptionMessage.Should().Be("innermessage");
            result.ExceptionType.Should().Be(Sql.ExceptionType.NoAccountNumberMatchDoubleSiret);
            result.CreationDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
        }
    }
}
