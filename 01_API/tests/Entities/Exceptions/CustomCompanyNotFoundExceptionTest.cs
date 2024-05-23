// <copyright file="CustomCompanyNotFoundExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests.Exceptions
{
    public class CustomCompanyNotFoundExceptionTest
    {
        [Fact]
        public void Constructor_Empty()
        {
            var ex = new CustomCompanyNotFoundException();
            ex.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.CustomCompanyNotFoundException' was thrown.");
        }

        [Fact]
        public void Constructor_Message()
        {
            var ex = new CustomCompanyNotFoundException(ExceptionType.AccountNumberNoMatchSiret, "message");
            ex.Message.Should().Be("message");
            ex.Type.Should().Be(ExceptionType.AccountNumberNoMatchSiret);
        }
    }
}
