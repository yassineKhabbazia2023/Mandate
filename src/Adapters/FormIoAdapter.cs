// <copyright file="FormIoAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client;

    public class FormIoAdapter : IFormIoService
    {
        private readonly IFormioClient formioClient;

        public FormIoAdapter(IFormioClient formioClient)
        {
            this.formioClient = formioClient;
        }

        public async Task<Collection?> GetSubmissionMandateAsync(Bban bban)
        {
            FormioAuthToken token = new FormioAuthToken()
            {
                Type = FormioTokenType.App,
            };

            FormioSubmissionCollection submission = await this.formioClient.GetSubmissionMandateAsync(
                bban.BankCode,
                bban.BranchCode,
                bban.AccountNumber,
                bban.CheckDigits,
                token);

            return submission.Submissions.Any() ? submission.ToCollection() : null;
        }
    }
}
