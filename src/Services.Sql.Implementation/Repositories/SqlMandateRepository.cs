// <copyright file="SqlMandateRepository.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation
{
    using Microsoft.Extensions.Options;

    public class SqlMandateRepository : IMandateRepository
    {
        private readonly IOptions<SqlMandateRepositoryOptions> options;

        public SqlMandateRepository(IOptions<SqlMandateRepositoryOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Value.Validate();
            this.options = options;
        }

        public async Task<CompanyDb> GetCompanyBySiretAsync(string siret)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<CollectionDb>> FindCollectionsAsync(CollectionQuery query)
        {
            using var context = new MandateContext(this.options);

            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
