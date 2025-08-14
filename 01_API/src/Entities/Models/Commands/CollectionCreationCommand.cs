// <copyright file="CollectionCreationCommand.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class CollectionCreationCommand
    {
        public CollectionCreationCommand(string erpId, Signatory signatory, Address address, Bban bban, string? destinationTool = null)
        {
            this.ErpId = erpId;
            this.Signatory = signatory;
            this.Address = address;
            this.Bban = bban;
            this.DestinationTool = destinationTool;
        }

        public string ErpId { get; }

        public Signatory Signatory { get; }

        public Address Address { get; }

        public Bban Bban { get; }

        public string? DestinationTool { get; }
    }
}