// <copyright file="CustomExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests.Exceptions
{
    public class CustomExceptionTest
    {
        [Fact]
        public void Constructor_Empty()
        {
            var ex = new CustomException();
            ex.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.CustomException' was thrown.");
            ex.Type.Should().Be(ExceptionType.AccountNumberMatchDoubleSiret);
        }

        [Fact]
        public void Constructor_InitialiseType()
        {
            var ex = new CustomException(ExceptionType.AccountNumberMatchDoubleSiret);
            ex.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.CustomException' was thrown.");
            ex.Type.Should().Be(ExceptionType.AccountNumberMatchDoubleSiret);
        }

        [Fact]
        public void Constructor_Message()
        {
            var ex = new CustomException(ExceptionType.AccountNumberNoMatchSiret, "message");
            ex.Message.Should().Be("message");
            ex.Type.Should().Be(ExceptionType.AccountNumberNoMatchSiret);
        }
    }
}
