// <copyright file="PortalClientExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Constellation.Portal.Client;

    public class PortalClientExtensionsTest
    {
        [Fact]
        public void ToModel()
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

            var entity = new AccountsWithoutCacheResponseJson
            {
                Id = new Guid("00000001-0000-0000-0000-000000000000"),
                AccountCategory = "category",
                AccountDeliveryAddress2 = "account delivery",
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

            var result = entity.ToModel();

            Signatory? signatory = new Signatory("M", "maroo", "elleuch", "maroo@email.com");
            Address? address = new Address("Bâtiment D20 - ZAC", "account delivery", "BP 54", "RAISMES", "France");
            var expectedCompany = new Company(
                    new Guid("00000001-0000-0000-0000-000000000000"),
                    "name",
                    "12345678901234",
                    "12345678910",
                    null,
                    signatory,
                    address);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedCompany);
        }
    }
}
