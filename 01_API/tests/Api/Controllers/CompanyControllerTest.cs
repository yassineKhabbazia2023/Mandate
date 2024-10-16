// <copyright file="CompanyControllerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class CompanyControllerTest
    {
        private readonly Mock<ILogger<CompanyController>> mockLogger;
        private readonly Mock<ICompanyManager> mockCompanyManager;
        private readonly CompanyController controller;

        public CompanyControllerTest()
        {
            this.mockLogger = new Mock<ILogger<CompanyController>>();
            this.mockCompanyManager = new Mock<ICompanyManager>();
            this.controller = new CompanyController(this.mockLogger.Object, this.mockCompanyManager.Object);
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_ReturnsOk_WhenCompanyFound()
        {
            // Arrange
            string testErpId = "testErpId";
            var userEmail = "user@email.test";

            // Create Signatory and Address objects
            var signatory = new Signatory("Mr.", "John", "Doe", "john.doe@example.com");
            var address = new Address("123 Main St", "Apt 4B", "12345", "New York", "USA");

            // Create Company object
            var expectedCompany = new Company(1, "Example Company", "12345678901234", testErpId, "BSP1234", signatory, address);


            this.mockCompanyManager
                .Setup(m => m.GetCompanyByErpIdAsync(testErpId, userEmail))
                .ReturnsAsync(expectedCompany);

            // Act
            var result = await this.controller.GetCompanyByErpIdAsync(testErpId, userEmail);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.StatusCode.Should().Be(200);
            var company = okResult.Value.Should().BeAssignableTo<Company>().Subject;
            company.Should().BeEquivalentTo(expectedCompany);
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_ReturnsNotFound_WhenCompanyNotFound()
        {
            // Arrange
            string testErpId = "nonExistingErpId";
            string userEmail = "user@email.test";

            this.mockCompanyManager
                .Setup(m => m.GetCompanyByErpIdAsync(testErpId, userEmail))
                .ThrowsAsync(new CompanyNotFoundException("Company not found"));

            // Act
            var result = await this.controller.GetCompanyByErpIdAsync(testErpId, userEmail);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetCompanyByErpIdAsync_ReturnsInternalServerError_OnException()
        {
            // Arrange
            string testErpId = "testErpId";
            string userEmail = "user@email.test";


            this.mockCompanyManager
                .Setup(m => m.GetCompanyByErpIdAsync(testErpId, userEmail))
                .ThrowsAsync(new Exception("Internal server error"));

            // Act
            var result = await this.controller.GetCompanyByErpIdAsync(testErpId, userEmail);

            // Assert
            var internalServerErrorResult = result.Should().BeOfType<ObjectResult>().Subject;
            internalServerErrorResult.StatusCode.Should().Be(500);
        }
    }
}
