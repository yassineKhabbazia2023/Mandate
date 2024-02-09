// <copyright file="CompanyManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.Managers
{
    public class CompanyManagerTest
    {
        [Fact]
        public async Task GetCompanyByErpIdAsync_ShouldReturnCompany_WhenErpIdIsValid()
        {
            // Arrange
            var erpId = "ERP001";
            var signatory = new Signatory("Mr.", "John", "Doe", "john.doe@example.com");
            var address = new Address("123 Main St", "Apt 4B", "12345", "Sample City", "ExampleLand");

            var expectedCompany = new Company(
                Guid.NewGuid(),
                "Example Company",
                "123456789",
                "ERP001",
                "JDF123",
                signatory,
                address);

            var databaseServiceMock = new Mock<IDatabaseService>();
            databaseServiceMock.Setup(service => service.GetCompanyByErpIdAsync(erpId))
                               .ReturnsAsync(expectedCompany)
                               .Verifiable();

            var companyManager = new CompanyManager(databaseServiceMock.Object);

            // Act
            var result = await companyManager.GetCompanyByErpIdAsync(erpId);

            // Assert
            result.Should().BeEquivalentTo(expectedCompany, options => options.ComparingByMembers<Company>());
            databaseServiceMock.Verify(service => service.GetCompanyByErpIdAsync(erpId), Times.Once);
        }
    }
}
