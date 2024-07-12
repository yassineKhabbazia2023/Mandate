// <copyright file="ErrorTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class ErrorTest
    {
        [Fact]
        public void Constructor()
        {
            var error = new Error("CompanyNotFound", "11111111-1111-1111-1111-111111111111", "Company not found");

            error.ErrorType.Should().Be("CompanyNotFound");
            error.LogReference.Should().Be("11111111-1111-1111-1111-111111111111");
            error.Message.Should().Be("Company not found");
        }

        [Fact]
        public void Serialization()
        {
            var error = new Error("CompanyNotFound", "11111111-1111-1111-1111-111111111111", "Company not found");

            error.Should().BeJsonSerializableTo(new
            {
                errorType = "CompanyNotFound",
                logReference = "11111111-1111-1111-1111-111111111111",
                message = "Company not found",
            });
        }
    }
}
