// <copyright file="PdfHelperTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    public class PdfHelperTest
    {
        [Fact]
        public async Task GeneratePdfFromTemplateAsync_NullSource_ThrowsArgumentNullException()
        {
            var pdfHelper = new PdfHelper(Mock.Of<IDatabaseService>(), Mock.Of<IPdfTextReplacer>());

            Func<Task> act = () => pdfHelper.GeneratePdfFromTemplateAsync(null!);
            await act.Should().ThrowExactlyAsync<ArgumentNullException>().WithParameterName("source");
        }

        [Fact]
        public async Task GeneratePdfFromTemplateAsync_NullBban_ThrowsArgumentNullException()
        {
            var pdfHelper = new PdfHelper(Mock.Of<IDatabaseService>(), Mock.Of<IPdfTextReplacer>());

            var collectionSource = new Collection(
                Guid.Empty,
                null,
                null,
                null,
                DateTime.Now,
                DateTime.Now,
                new Status(CollectionStatus.ToDo, "todo", Mandate.JdcCollectionStatus.Creation_InProgress, null), null);

            Func<Task> act = () => pdfHelper.GeneratePdfFromTemplateAsync(collectionSource);
            await act.Should().ThrowExactlyAsync<ArgumentNullException>().WithParameterName("source.Bban");
        }

        [Fact]
        public async Task GeneratePdfFromTemplateAsync_Success()
        {
            var templateBytes = new byte[] { 1, 2, 3 };
            var expectedPdf = new byte[] { 4, 5, 6 };

            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            databaseService
                .Setup(m => m.GetPdfTemplateByBankCodeAsync("12345"))
                .ReturnsAsync(templateBytes)
                .Verifiable();

            var textReplacer = new Mock<IPdfTextReplacer>(MockBehavior.Strict);
            textReplacer
                .Setup(m => m.ReplaceText(templateBytes, It.Is<Replacement[]>(r => r.Length == 13)))
                .Returns(expectedPdf)
                .Verifiable();

            var pdfHelper = new PdfHelper(databaseService.Object, textReplacer.Object);

            var collectionSource = new Collection(
                Guid.NewGuid(),
                null,
                new Company(
                    1,
                    "Test Company",
                    "12345678901234",
                    "123456789",
                    string.Empty,
                    new Signatory(null, "Jean", "Dupont", null),
                    new Address("10 rue de la Paix", "Bat A", "75002", "Paris", "France")),
                new Bban(
                    "12345",
                    "67890",
                    "12345678901",
                    "55",
                    string.Empty,
                    new Bank(
                        "12345",
                        "Test Bank",
                        string.Empty,
                        string.Empty,
                        EntityFactory.BankAgreement)),
                DateTime.Now,
                DateTime.Now,
                new Status(CollectionStatus.ToDo, "todo", Mandate.JdcCollectionStatus.Creation_InProgress, null),
                null);

            var result = await pdfHelper.GeneratePdfFromTemplateAsync(collectionSource);

            result.Should().BeEquivalentTo(expectedPdf);
            databaseService.VerifyAll();
            textReplacer.VerifyAll();
        }

        [Fact]
        public async Task GeneratePdfFromTemplateAsync_NullCompanyAndNullBank_UsesEmptyStrings()
        {
            var templateBytes = new byte[] { 1, 2, 3 };
            var expectedPdf = new byte[] { 4, 5, 6 };

            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            databaseService
                .Setup(m => m.GetPdfTemplateByBankCodeAsync("12345"))
                .ReturnsAsync(templateBytes)
                .Verifiable();

            var textReplacer = new Mock<IPdfTextReplacer>(MockBehavior.Strict);
            textReplacer
                .Setup(m => m.ReplaceText(templateBytes, It.Is<Replacement[]>(r =>
                    r.Length == 13
                    && r[4].Value == string.Empty   // bankName
                    && r[5].Value == string.Empty   // companyName
                    && r[6].Value == string.Empty   // siren
                    && r[7].Value == string.Empty   // companyAddressStreet
                    && r[8].Value == string.Empty   // companyAddressZipCode
                    && r[9].Value == string.Empty   // companyAddressCountry
                    && r[10].Value == string.Empty  // companyAddressComplements
                    && r[11].Value == string.Empty  // signatory
                    && r[12].Value == string.Empty  // companyAddress
                )))
                .Returns(expectedPdf)
                .Verifiable();

            var pdfHelper = new PdfHelper(databaseService.Object, textReplacer.Object);

            var collectionSource = new Collection(
                Guid.NewGuid(),
                null,
                null,
                new Bban("12345", "67890", "12345678901", "55", string.Empty, null),
                DateTime.Now,
                DateTime.Now,
                new Status(CollectionStatus.ToDo, "todo", Mandate.JdcCollectionStatus.Creation_InProgress, null),
                null);

            var result = await pdfHelper.GeneratePdfFromTemplateAsync(collectionSource);

            result.Should().BeEquivalentTo(expectedPdf);
            databaseService.VerifyAll();
            textReplacer.VerifyAll();
        }

        [Fact]
        public async Task GeneratePdfFromTemplateAsync_NullAddressAndSignatory_UsesEmptyStrings()
        {
            var templateBytes = new byte[] { 1, 2, 3 };
            var expectedPdf = new byte[] { 4, 5, 6 };

            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            databaseService
                .Setup(m => m.GetPdfTemplateByBankCodeAsync("12345"))
                .ReturnsAsync(templateBytes)
                .Verifiable();

            var textReplacer = new Mock<IPdfTextReplacer>(MockBehavior.Strict);
            textReplacer
                .Setup(m => m.ReplaceText(templateBytes, It.Is<Replacement[]>(r =>
                    r.Length == 13
                    && r[4].Value == "Test Bank"      // bankName present
                    && r[5].Value == "Test Company"    // companyName present
                    && r[6].Value == "12345678901234"  // siren present
                    && r[7].Value == string.Empty      // companyAddressStreet empty (null address)
                    && r[8].Value == string.Empty      // companyAddressZipCode
                    && r[9].Value == string.Empty      // companyAddressCountry
                    && r[10].Value == string.Empty     // companyAddressComplements
                    && r[11].Value == string.Empty     // signatory empty (null signatory)
                    && r[12].Value == string.Empty     // companyAddress
                )))
                .Returns(expectedPdf)
                .Verifiable();

            var pdfHelper = new PdfHelper(databaseService.Object, textReplacer.Object);

            var collectionSource = new Collection(
                Guid.NewGuid(),
                null,
                new Company(1, "Test Company", "12345678901234", "123456789", string.Empty, null, null),
                new Bban(
                    "12345", "67890", "12345678901", "55", string.Empty,
                    new Bank("12345", "Test Bank", string.Empty, string.Empty, EntityFactory.BankAgreement)),
                DateTime.Now,
                DateTime.Now,
                new Status(CollectionStatus.ToDo, "todo", Mandate.JdcCollectionStatus.Creation_InProgress, null),
                null);

            var result = await pdfHelper.GeneratePdfFromTemplateAsync(collectionSource);

            result.Should().BeEquivalentTo(expectedPdf);
            databaseService.VerifyAll();
            textReplacer.VerifyAll();
        }
    }
}
