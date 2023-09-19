// <copyright file="RefPdfTemplateDbTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    public class RefPdfTemplateDbTest
    {
        [Fact]
        public void Defaults()
        {
            var entity = new RefPdfTemplateDb();

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(2);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.BankCode.Should().Be(string.Empty);
            entity.PdfFile.Should().BeNull();
        }

        [Fact]
        public void Values()
        {
            var entity = new RefPdfTemplateDb()
            {
                BankCode = "bc",
                PdfFile = Encoding.UTF8.GetBytes("file"),
            };

            entity.BankCode.Should().Be("bc");
            entity.PdfFile.Length.Should().Be(4);
        }
    }
}
