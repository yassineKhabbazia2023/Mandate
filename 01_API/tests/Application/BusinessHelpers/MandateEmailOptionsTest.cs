// <copyright file="MandateEmailOptionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.BusinessHelpers
{
    public class MandateEmailOptionsTest
    {
        [Fact]
        public void Properties_ShouldBeSetAndGetCorrectly()
        {
            // Arrange
            var options = new MandateEmailOptions
            {
                MandateCancellationSubject = "Cancellation Subject",
                MandateCancellationTemplateName = "Cancellation Template",
                MandateCancellationFromEmail = "cancel_from@example.com",
                MandateCancellationToEmail = "cancel_to@example.com",
                MandateCancellationCcEmails = new List<string> { "cancel_cc1@example.com", "cancel_cc2@example.com" },
                MandateUploadedSubject = "Uploaded Subject",
                MandateUploadedTemplateName = "Uploaded Template",
                MandateUploadedFromEmail = "upload_from@example.com",
                MandateUploadedToEmail = "upload_to@example.com",
                MandateUploadedCcEmails = new List<string> { "upload_cc1@example.com", "upload_cc2@example.com" },
            };

            // Act & Assert
            options.MandateCancellationSubject.Should().Be("Cancellation Subject");
            options.MandateCancellationTemplateName.Should().Be("Cancellation Template");
            options.MandateCancellationFromEmail.Should().Be("cancel_from@example.com");
            options.MandateCancellationToEmail.Should().Be("cancel_to@example.com");
            options.MandateCancellationCcEmails.Should().BeEquivalentTo(new List<string> { "cancel_cc1@example.com", "cancel_cc2@example.com" });
            options.MandateUploadedSubject.Should().Be("Uploaded Subject");
            options.MandateUploadedTemplateName.Should().Be("Uploaded Template");
            options.MandateUploadedFromEmail.Should().Be("upload_from@example.com");
            options.MandateUploadedToEmail.Should().Be("upload_to@example.com");
            options.MandateUploadedCcEmails.Should().BeEquivalentTo(new List<string> { "upload_cc1@example.com", "upload_cc2@example.com" });
        }
    }
}
