// <copyright file="Company.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>


namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Company
    {
        private string? bankServicesProviderId;

        public Company(int id, string? name, string siretNumber, string? erpId, string? bankServicesProviderId, Signatory? signatory, Address? address)
        {
            this.Id = id;
            this.Name = name;
            this.SiretNumber = siretNumber;
            this.ErpId = erpId;
            this.bankServicesProviderId = bankServicesProviderId;
            this.Signatory = signatory;
            this.Address = address;
        }

        public int Id { get; }

        public string? Name { get; }

        public string SiretNumber { get; }

        public string? ErpId { get; }

        // folderId
        public string? BankServicesProviderId
        {
            get
            {
                return this.bankServicesProviderId;
            }
        }

        public Signatory? Signatory { get; }

        public Address? Address { get; }

        public void SetBankServicesProviderId(string? jdcDossierId)
        {
            this.bankServicesProviderId = jdcDossierId;
        }
    }
}
