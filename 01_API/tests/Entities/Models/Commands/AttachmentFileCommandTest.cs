// <copyright file="AttachmentFileCommandTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class AttachmentFileCommandTest
    {
        [Fact]
        public void Constructor_SetsFileNameAndContent()
        {
            // Arrange
            var expectedFileName = "TestFile.txt";
            var expectedContent = "This is a test content.";

            // Act
            var attachment = new AttachmentFileCommand(expectedFileName, expectedContent);

            // Assert
            attachment.FileName.Should().Be(expectedFileName, because: "FileName should be set by the constructor.");
            attachment.Content.Should().Be(expectedContent, because: "Content should be set by the constructor.");
        }
    }
}
