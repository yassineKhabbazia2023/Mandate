// <copyright file="PortalProviderTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Mandate.Services.Portal.Implementation.Tests
{
    using FluentAssertions;
    using KPMG.Constellation.Portal.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.PortalApi;
    using Moq;
    using Xunit;

    public class PortalProviderTest
    {
        [Fact]
        public void Constructor()
        {
            var provider = new PortalProvider(
                Mock.Of<IPortalClientFactory>(MockBehavior.Strict),
                Mock.Of<IAuthenticationContext>(MockBehavior.Strict));

            provider.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAccountsODataWithoutCache_Should_Return_Data()
        {
            var contact = new ContactResponseJson()
            {
                Id = new Guid("00000002-0000-0000-0000-000000000000"),
                ConactFirstName = "maroo",
                ConactLastName = "elleuch",
                ContactTitle = "M",
                LoginName = "maroo@email.com",
            };

            var roles = new List<RoleResponseJson>()
            {
                new RoleResponseJson
                {
                    RoleFunctionName = "roleName",
                    Contact = contact,
                },
            };

            var account = new AccountsWithoutCacheResponseJson
            {
                Id = new Guid("00000001-0000-0000-0000-000000000000"),
                AccountCategory = "category",
                AccountDeliveryAddress2 = string.Empty,
                AccountDeliveryEmail = "maroo@email.com",
                AccountName = "name",
                AccountRegisterIdentification1 = "12345678901234",
                Adresse = "Bâtiment D20 - ZAC",
                Country = "France",
                IBSCode = "12345678910",
                State = "RAISMES",
                ZipCode = "BP 54",
                Roles = roles,
            };

            var accounts = new AccountWithoutCacheRootResponseJson
            {
                Odatacount = "1",
                Odatacontext = "http://xxx",
                Accounts = new List<AccountsWithoutCacheResponseJson>()
                {
                    account,
                },
            };

            string query = "$top=10&$skip=0&$filter=xxx&$expend=xx&$count=true";
            var portalClient = new Mock<IPortalClient>(MockBehavior.Strict);
            portalClient.Setup(i => i.GetAccountsODataWithoutCache(query))
                .ReturnsAsync(accounts)
                .Verifiable();

            var authenticationContext = new Mock<IAuthenticationContext>(MockBehavior.Strict);
            authenticationContext.Setup(i => i.BearerToken).Returns("abc").Verifiable();

            var factory = new Mock<IPortalClientFactory>(MockBehavior.Strict);
            factory.Setup(i => i.Create("abc"))
                .Returns(portalClient.Object)
                .Verifiable();

            var provider = new PortalProvider(factory.Object, authenticationContext.Object);
            var result = await provider.GetAccountsODataWithoutCache(10, 0, "$filter=xxx&$expend=xx", true);

            AccountWithoutCacheRootResponseJson expectedResult = new AccountWithoutCacheRootResponseJson
            {
                Odatacount = "1",
                Odatacontext = "http://xxx",
                Accounts = new List<AccountsWithoutCacheResponseJson>()
                {
                    new AccountsWithoutCacheResponseJson
                    {
                        Id = new Guid("00000001-0000-0000-0000-000000000000"),
                        AccountCategory = "category",
                        AccountDeliveryAddress2 = string.Empty,
                        AccountDeliveryEmail = "maroo@email.com",
                        AccountName = "name",
                        AccountRegisterIdentification1 = "12345678901234",
                        Adresse = "Bâtiment D20 - ZAC",
                        Country = "France",
                        IBSCode = "12345678910",
                        State = "RAISMES",
                        ZipCode = "BP 54",
                        Roles = new List<RoleResponseJson>()
                        {
                            new RoleResponseJson
                            {
                                RoleFunctionName = "roleName",
                                Contact =  new ContactResponseJson()
                                {
                                    Id = new Guid("00000002-0000-0000-0000-000000000000"),
                                    ConactFirstName = "maroo",
                                    ConactLastName = "elleuch",
                                    ContactTitle = "M",
                                    LoginName = "maroo@email.com",
                                },
                            },
                        },
                    },
                },
            };

            result.Should().BeEquivalentTo(expectedResult);
            portalClient.VerifyAll();
            authenticationContext.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task GetAccountsODataWithoutCache_Should_Return_Exp_When_Client_Exp()
        {
            var portalClient = new Mock<IPortalClient>(MockBehavior.Strict);
            portalClient.Setup(i => i.GetAccountsODataWithoutCache(It.IsAny<string>()))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            var authenticationContext = new Mock<IAuthenticationContext>(MockBehavior.Strict);
            authenticationContext.Setup(i => i.BearerToken).Returns("abc").Verifiable();

            var factory = new Mock<IPortalClientFactory>(MockBehavior.Strict);
            factory.Setup(i => i.Create("abc"))
                .Returns(portalClient.Object)
                .Verifiable();

            var provider = new PortalProvider(factory.Object, authenticationContext.Object);

            Func<Task> action = async () => await provider.GetAccountsODataWithoutCache(10, 0, string.Empty, false);
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            portalClient.VerifyAll();
            authenticationContext.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task GetAccountsODataWithoutCache_Should_Return_Exp_When_Factory_Exp()
        {
            var portalClient = new Mock<IPortalClient>(MockBehavior.Strict);

            var authenticationContext = new Mock<IAuthenticationContext>(MockBehavior.Strict);
            authenticationContext.Setup(i => i.BearerToken).Returns("abc").Verifiable();

            var factory = new Mock<IPortalClientFactory>(MockBehavior.Strict);
            factory.Setup(i => i.Create("abc"))
                .Throws(new Exception("message"))
                .Verifiable();

            var provider = new PortalProvider(factory.Object, authenticationContext.Object);

            Func<Task> action = async () => await provider.GetAccountsODataWithoutCache(10, 0, string.Empty, false);
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            portalClient.VerifyAll();
            authenticationContext.VerifyAll();
            factory.VerifyAll();
        }
    }
}
