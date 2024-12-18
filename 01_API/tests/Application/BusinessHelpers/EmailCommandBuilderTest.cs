// <copyright file="EmailCommandBuilderTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    public class EmailCommandBuilderTest
    {
        [Fact]
        public void CreateMandateCancellationEmail_ShouldReturnCorrectEmailCommand()
        {
            // Arrange
            var collectionId = new PredictableGuid().NewGuid();
            var companyId = 1;

            var company = TestHelper.GetCompany(companyId, "bankServicesProviderId");
            Bban bban = TestHelper.GetBban("ebicsCardId", false);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                collectionId,
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var options = new MandateEmailOptions
            {
                MandateCancellationSubject = "Cancellation Subject",
                MandateCancellationTemplateName = "Cancellation Template",
                MandateCancellationFromEmail = "cancel_from@example.com",
                MandateCancellationToEmail = "cancel_to@example.com",
                MandateCancellationCcEmails = new List<string> { "cancel_cc1@example.com", "cancel_cc2@example.com" },
            };

            // Act
            var emailCommand = EmailCommandBuilder.CreateMandateCancellationEmail(collection, options);

            // Assert
            emailCommand.Should().NotBeNull();
            emailCommand.Subject.Should().Contain("Cancellation Subject");
            emailCommand.TemplateName.Should().Be("Cancellation Template");
            emailCommand.From.Should().Be("cancel_from@example.com");
            emailCommand.To.Should().Be("cancel_to@example.com");
            emailCommand.Cc.Should().BeEquivalentTo(options.MandateCancellationCcEmails);
            emailCommand.Attachements.Should().BeEmpty();
            emailCommand.Variables.Should().ContainKeys("collaboratorEmail", "siretNumber", "accountHolder", "bankName", "bankCode", "accountNumber", "branchCode", "checkDigits", "companyName");
        }

        [Fact]
        public void CreateSignedMandateUploadedEmail_ShouldReturnCorrectEmailCommand()
        {
            // Arrange
            var collectionId = new PredictableGuid().NewGuid();
            var companyId = 1;

            var company = TestHelper.GetCompany(companyId, "bankServicesProviderId");
            Bban bban = TestHelper.GetBban("ebicsCardId", false);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                collectionId,
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var options = new MandateEmailOptions
            {
                MandateUploadedSubject = "Uploaded Subject",
                MandateUploadedTemplateName = "Uploaded Template",
                MandateUploadedFromEmail = "upload_from@example.com",
                MandateUploadedToEmail = "upload_to@example.com",
                MandateUploadedCcEmails = new List<string> { "upload_cc1@example.com", "upload_cc2@example.com" },
            };
            string fileContent = "file content";
            string fileName = "file.pdf";

            // Act
            var emailCommand = EmailCommandBuilder.CreateSignedMandateUploadedEmail(collection, options, fileContent, fileName);

            // Assert
            emailCommand.Should().NotBeNull();
            emailCommand.Subject.Should().Contain("Uploaded Subject");
            emailCommand.TemplateName.Should().Be("Uploaded Template");
            emailCommand.From.Should().Be("upload_from@example.com");
            emailCommand.To.Should().Be("upload_to@example.com");
            emailCommand.Cc.Should().BeEquivalentTo(options.MandateUploadedCcEmails);
            emailCommand.Attachements.Should().HaveCount(1);
            emailCommand.Attachements[0].FileName.Should().Be(fileName);
            emailCommand.Variables.Should().ContainKeys("collaboratorEmail", "siretNumber", "accountHolder", "bankName", "bankCode", "accountNumber", "branchCode", "checkDigits", "companyName");
        }
    }
}
