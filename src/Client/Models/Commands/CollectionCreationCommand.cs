// <copyright file="CollectionCreationCommand.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class CollectionCreationCommand
    {
        [JsonConstructor]
        public CollectionCreationCommand(string erpId, Signatory? signatory, Address? address, Bban? bban)
        {
            this.ErpId = erpId;
            this.Signatory = signatory;
            this.Address = address;
            this.Bban = bban;
        }

        [JsonProperty("erpId")]
        public string ErpId { get; }

        [JsonProperty("signatory")]
        public Signatory? Signatory { get; }

        [JsonProperty("address")]
        public Address? Address { get; }

        [JsonProperty("bban")]
        public Bban? Bban { get; }
    }
}
