// <copyright file="FormIoAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client;

    public class FormIoAdapter : IFormIoService
    {
        private readonly IFormIoClient formioClient;

        public FormIoAdapter(IFormIoClient formioClient)
        {
            this.formioClient = formioClient;
        }

        public async Task<Collection?> GetSubmissionMandateAsync(Bban bban)
        {
            FormIoAuthToken token = new FormIoAuthToken()
            {
                Type = FormIoTokenType.App,
            };

            FormIoSubmissionCollection submission = await this.formioClient.GetSubmissionMandateAsync(
                bban?.BankCode!,
                bban?.BranchCode!,
                bban?.AccountNumber!,
                bban?.CheckDigits!,
                token);

            return submission.Submissions.Any() ? submission.ToCollection() : null;
        }
    }
}
