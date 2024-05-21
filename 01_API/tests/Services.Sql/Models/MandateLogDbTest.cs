// <copyright file="MandateLogDbTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests.Models
{
    public class MandateLogDbTest
    {
        [Fact]
        public void Values()
        {
            var entity = new MandateLogDb()
            {
                Id = new PredictableGuid(100).NewGuid(),
                SiretNumber = "12345678901234",
                ErpId = "1234567890",
                BankCode = "12345",
                AccountNumber = "12345678910",
                BranchCode = "54321",
                CheckDigits = "11",
                JdcDossierId = "1234",
                JdcReleveId = "4321",
                JdcRibId = "4321",
                ExceptionType = ExceptionType.NoAccountNumberMatchDoubleSiret,
                ExceptionMessage = "message",
                InnerExceptionMessage = "innermessage",
                CreationDate = DateTime.Now,
            };

            entity.GetType().GetProperties().Length.Should().Be(14);

            // Assert
            entity.Id.Should().Be(new PredictableGuid(100).NewGuid());
            entity.SiretNumber.Should().Be("12345678901234");
            entity.ErpId.Should().Be("1234567890");
            entity.BankCode.Should().Be("12345");
            entity.AccountNumber.Should().Be("12345678910");
            entity.BranchCode.Should().Be("54321");
            entity.CheckDigits.Should().Be("11");
            entity.JdcDossierId.Should().Be("1234");
            entity.JdcReleveId.Should().Be("4321");
            entity.JdcRibId.Should().Be("4321");
            entity.ExceptionMessage.Should().Be("message");
            entity.InnerExceptionMessage.Should().Be("innermessage");
            entity.ExceptionType.Should().Be(Sql.ExceptionType.NoAccountNumberMatchDoubleSiret);
            entity.CreationDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
        }
    }
}
