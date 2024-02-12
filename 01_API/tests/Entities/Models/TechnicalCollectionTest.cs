// <copyright file="TechnicalCollectionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class TechnicalCollectionTest
    {
        [Fact]
        public void Constructor()
        {
            // Arrange
            var id = Guid.NewGuid();
            var folderId = "TestFolderId";
            var ribId = "TestRibId";
            var bankCode = "TestBankCode";
            var branchCode = "TestBranchCode";
            var accountNumber = "TestAccountNumber";
            var checkDigits = "TestCheckDigits";
            var statusCode = "TestStatusCode";

            // Act
            var entity = new TechnicalCollection(
                id,
                folderId,
                ribId,
                new BankDetails(
                    bankCode,
                    branchCode,
                    accountNumber,
                    checkDigits),
                statusCode);

            // Assert
            entity.GetType().GetProperties().Length.Should().Be(5);
            entity.Id.Should().Be(id);
            entity.FolderId.Should().Be(folderId);
            entity.RibId.Should().Be(ribId);
            entity.BankDetails.BankCode.Should().Be(bankCode);
            entity.BankDetails.BranchCode.Should().Be(branchCode);
            entity.BankDetails.AccountNumber.Should().Be(accountNumber);
            entity.BankDetails.CheckDigits.Should().Be(checkDigits);
            entity.StatusCode.Should().Be(statusCode);
        }
    }
}
