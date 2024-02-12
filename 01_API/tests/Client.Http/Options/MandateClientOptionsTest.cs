// <copyright file="MandateClientOptionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Options;

    public class MandateClientOptionsTest
    {
        [Theory]
        [InlineData("http://example.com", "http://example.com/")]
        [InlineData("http://example.com/", "http://example.com/")]
        public void BaseUri_ShouldAppendTrailingSlashIfMissing(string inputUri, string expectedUri)
        {
            // Arrange
            var options = new MandateClientOptions();

            // Act
            options.BaseUri = new Uri(inputUri);

            // Assert
            options.BaseUri.ToString().Should().Be(expectedUri);
        }

        [Fact]
        public void Validate_ThrowsInvalidOperationException_WhenBaseUriIsNull()
        {
            // Arrange
            var options = new MandateClientOptions();

            // Act & Assert
            Action act = () => options.Validate();
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*BaseUri*is null"); // Checks if the exception message contains specific text
        }

        [Fact]
        public void Validate_DoesNotThrow_WhenBaseUriIsValid()
        {
            // Arrange
            var options = new MandateClientOptions
            {
                BaseUri = new Uri("http://example.com"),
            };

            // Act & Assert
            Action act = () => options.Validate();
            act.Should().NotThrow();
        }
    }
}
