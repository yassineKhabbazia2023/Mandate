// <copyright file="FormioAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using System.Collections.Generic;
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client;

    public class FormioAdapter : IFormioService
    {
        private readonly IFormioClient formioClient;

        public FormioAdapter(IFormioClient formioClient)
        {
            this.formioClient = formioClient;
        }

        public async Task<List<Collection>> GetAllCollectionAsync(int skip, int limit)
        {
            FormioAuthToken token = new FormioAuthToken()
            {
                Type = FormioTokenType.App,
            };

            FormioSubmissionCollection? submissions = await this.formioClient.GetSubmissionsAsync(skip, limit, token);

            return submissions?
                .Submissions?
                .Select(item => item.ToCollection())
                .ToList() !;
        }

        public async Task<Collection?> GetSubmissionMandateAsync(Bban bban)
        {
            FormioAuthToken token = new FormioAuthToken()
            {
                Type = FormioTokenType.App,
            };

            FormioSubmissionCollection submission = await this.formioClient.GetSubmissionMandateAsync(
                bban?.BankCode!,
                bban?.BranchCode!,
                bban?.AccountNumber!,
                bban?.CheckDigits!,
                token);

            return submission.Submissions.Any() ? submission.Submissions.SingleOrDefault() !.ToCollection() : null;
        }
    }
}
