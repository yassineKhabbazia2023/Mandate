// <copyright file="MandateCreationDto.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class MandateCreation
    {
        public MandateCreation(string erpId, Signatory signatory, Address address, Bban bban)
        {
            this.ErpId = erpId;
            this.Signatory = signatory;
            this.Address = address;
            this.Bban = bban;
        }

        public string ErpId { get; }

        public Signatory Signatory { get; }

        public Address Address { get; }

        public Bban Bban { get; }
    }
}