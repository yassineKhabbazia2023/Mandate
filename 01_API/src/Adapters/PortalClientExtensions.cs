// <copyright file="PortalClientExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Constellation.Portal.Client;
    using KPMG.Pulse.Back.Accounting.Mandate;

    public static class PortalClientExtensions
    {
        public static Company ToModel(this AccountsWithoutCacheResponseJson source)
        {
            var signatory = source.Roles?
                .Single(x => x.Contact != null && x.Contact.LoginName == source.AccountDeliveryEmail)
                .Contact;

            return new Company(
                default,
                source.AccountName,
                source.AccountRegisterIdentification1,
                source.IBSCode,
                null,
                new Signatory(signatory?.ContactTitle, signatory?.ConactFirstName, signatory?.ConactLastName, signatory?.LoginName),
                new Address(source.Adresse, source.AccountDeliveryAddress2, source.ZipCode, source.State, source.Country));
        }
    }
}
