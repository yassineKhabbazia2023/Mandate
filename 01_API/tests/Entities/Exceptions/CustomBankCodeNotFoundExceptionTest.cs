// <copyright file="CustomBankCodeNotFoundExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests.Exceptions
{
    public class CustomBankCodeNotFoundExceptionTest
    {
        [Fact]
        public void Constructor_Empty()
        {
            var ex = new CustomBankCodeNotFoundException();
            ex.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.CustomBankCodeNotFoundException' was thrown.");
        }

        [Fact]
        public void Constructor_Message()
        {
            var ex = new CustomBankCodeNotFoundException(ExceptionType.BankCodeNotFound, "message");
            ex.Message.Should().Be("message");
            ex.Type.Should().Be(ExceptionType.BankCodeNotFound);
        }

        [Fact]
        public void Constructor_Custom()
        {
            var ex = CustomBankCodeNotFoundException.FromId("12345");

            ex.Message.Should().Be("La banque avec le code '12345' n'a pas été trouvée dans le référentiel");
        }
    }
}
