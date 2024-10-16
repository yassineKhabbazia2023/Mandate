// <copyright file="MandateCreationMessage.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class MandateCreationMessage
    {
        public required int Id { get; init; }

        public string? Name { get; init; }

        public required string SiretNumber { get; init; }

        public string? ErpId { get; init; }

        // folderId
        public string? BankServicesProviderId { get; init; }

        public Signatory? Signatory { get; init; }

        public Address? Address { get; init; }

        public Bban Bban { get; init; }

        public Bank Bank { get; init; }

        public Company Company { get; set; }
    }
}
