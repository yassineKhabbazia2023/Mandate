// <copyright file="FormioOptionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http.Tests
{
    public class FormioOptionsTest
    {
        [Fact]
        public void Constructor()
        {
            var options = new FormioOptions();

            options.BaseUri.Should().BeNull();
            options.FormioApiKey.Should().BeNull();
        }

        [Fact]
        public void FormIoOptions_ValueChanged()
        {
            var options = new FormioOptions()
            {
                BaseUri = new Uri("https://toto.com"),
                FormioApiKey = "formioApiKeyT",
            };

            options.BaseUri.Should().BeEquivalentTo(new Uri("https://toto.com/"));
            options.FormioApiKey.Should().Be("formioApiKeyT");
        }

        [Fact]
        public void FormIoOptions_Validate_WhenBaseUriIsNull()
        {
            var options = new FormioOptions()
            {
                BaseUri = null,
                FormioApiKey = "formioApiKeyT",
            };

            var action = () => options.Validate();
            action.Should().Throw<Exception>().WithMessage("Instance of FormioOptions is invalid, BaseUri is null");
        }

        [Fact]
        public void FormIoOptions_Validate_WhenFormioApiKeyIsNull()
        {
            var options = new FormioOptions()
            {
                BaseUri = new Uri("https://toto.com"),
                FormioApiKey = null!,
            };

            var action = () => options.Validate();
            action.Should().Throw<Exception>().WithMessage("Instance of FormioOptions is invalid, FormioApiKey is null");
        }
    }
}
