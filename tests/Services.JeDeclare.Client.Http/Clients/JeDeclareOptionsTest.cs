// <copyright file="JeDeclareOptionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http.Tests
{
    public class JeDeclareOptionsTest
    {
        [Fact]
        public void Constructor()
        {
            var options = new JeDeclareOptions();

            options.BaseUri.Should().BeNull();
            options.Login.Should().BeNull();
            options.Password.Should().BeNull();
        }

        [Fact]
        public void JeDeclareOptions_ValueChanged()
        {
            var options = new JeDeclareOptions()
            {
                BaseUri = new Uri("https://toto.com"),
                Login = "loginT",
                Password = "passwordT",
            };

            options.BaseUri.Should().BeEquivalentTo(new Uri("https://toto.com/"));
            options.Login.Should().Be("loginT");
            options.Password.Should().Be("passwordT");
        }

        [Fact]
        public void JeDeclareOptions_Validate_WhenBaseUriIsNull()
        {
            var options = new JeDeclareOptions()
            {
                Login = "loginT",
                Password = "passwordT",
            };

            var action = () => options.Validate();
            action.Should().Throw<Exception>().WithMessage("Instance of JeDeclareOptions is invalid, BaseUri is null");
        }

        [Fact]
        public void JeDeclareOptions_Validate_WhenLoginIsNull()
        {
            var options = new JeDeclareOptions()
            {
                BaseUri = new Uri("https://toto.com"),
                Password = "passwordT",
            };

            var action = () => options.Validate();
            action.Should().Throw<Exception>().WithMessage("Instance of JeDeclareOptions is invalid, Login is null");
        }

        [Fact]
        public void JeDeclareOptions_Validate_WhenPasswordIsNull()
        {
            var options = new JeDeclareOptions()
            {
                BaseUri = new Uri("https://toto.com"),
                Login = "loginT",
            };

            var action = () => options.Validate();
            action.Should().Throw<Exception>().WithMessage("Instance of JeDeclareOptions is invalid, Password is null");
        }
    }
}
