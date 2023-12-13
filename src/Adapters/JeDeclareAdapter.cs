// <copyright file="JeDeclareAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;

    public class JeDeclareAdapter : IJeDeclareService
    {
        private readonly IJeDeclareClient jedeclareClient;

        public JeDeclareAdapter(IJeDeclareClient jedeclareClient)
        {
            this.jedeclareClient = jedeclareClient;
        }

        public Task<Bban> AddRibToFolderAsync(string? bankServicesProviderId, Bban bban, Signatory signatory)
        {
            throw new NotImplementedException();
        }

        public Task<Collection> CreateCollecteConfigurationAsync(string bankServicesProviderId, Bban rib)
        {
            throw new NotImplementedException();
        }

        public Task<Company> CreateFolderAsync(Company company)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> GetMandatPdfAsync(string jdcFolderId, string jdcRibId)
        {
            try
            {
                return await this.jedeclareClient.GetMandatPdfAsync(jdcFolderId, jdcRibId);
            }
            catch (JeDeclareApiException ex)
            {
                throw new ServicesProviderException(ex.Message, ex);
            }
        }

        public Task<string> UploadSignedMandate(Collection collection, byte[] mandateFile)
        {
            var folderId = collection?.Company?.BankServicesProviderId;
            var ribId = collection?.Bban?.BbanServicesProviderId;
            return this.jedeclareClient.UploadSignedMandat(folderId!, ribId!, mandateFile);
        }
    }
}