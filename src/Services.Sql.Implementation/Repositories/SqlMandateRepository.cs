// <copyright file="SqlMandateRepository.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation
{
    using Microsoft.EntityFrameworkCore;
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

        public async Task<(List<CollectionDb>, int)> SearchCollectionsAsync(CollectionQuery query)
        {
            using var context = new MandateContext(this.options);
            var mandates = context.Collection
                .Include(c => c.Bank)
                .Include(item => item.Company)
                .Include(item => item.Statuses).ThenInclude(item => item.RefStatusCode)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                mandates = mandates.Where(item =>
                    (item.AccountNumber == query.SearchTerm) ||
                    (item.Bank!.BankName == query.SearchTerm) ||
                    (item.Company!.Name == query.SearchTerm) ||
                    (item.Company!.ErpId == query.SearchTerm));
            }

            if (query.StatusCodes != null && query.StatusCodes.Any())
            {
                mandates = mandates.Where(item =>
                    item.Statuses.Any(s => s.IsCurrent && query.StatusCodes.Any(scp => s.RefStatusCode!.PulseCode == scp)));
            }

            if (query.CreationDateStart.HasValue && query.CreationDateEnd.HasValue)
            {
                mandates = mandates.Where(item =>
                    item.Statuses.Any(s =>
                        s.StatusCode == -1 &&
                        (s.StatusDate.HasValue &&
                        s.StatusDate.Value.Date >= query.CreationDateStart.Value.Date &&
                        s.StatusDate.Value.Date <= query.CreationDateEnd.Value.Date)));
            }

            if (query.ModificationDateStart.HasValue && query.ModificationDateEnd.HasValue)
            {
                mandates = mandates.Where(item =>
                    item.Statuses.Any(s =>
                        s.IsCurrent &&
                        (s.StatusDate.HasValue &&
                        s.StatusDate.Value.Date >= query.ModificationDateStart.Value.Date &&
                        s.StatusDate.Value.Date <= query.ModificationDateEnd.Value.Date)));
            }

            switch (query.SortCriteria, query.SortOrder)
            {
                case (CollectionSortCriteria.ErpId, SortOrder.Ascending):
                    mandates = mandates.OrderBy(item => item.Company!.ErpId);
                    break;

                case (CollectionSortCriteria.Name, SortOrder.Ascending):
                    mandates = mandates.OrderBy(item => item.Company!.Name);
                    break;

                case (CollectionSortCriteria.AccountNumber, SortOrder.Ascending):
                    mandates = mandates.OrderBy(item => item.AccountNumber);
                    break;

                case (CollectionSortCriteria.BankName, SortOrder.Ascending):
                    mandates = mandates.OrderBy(item => item.Bank!.BankName);
                    break;

                case (CollectionSortCriteria.CreationDate, SortOrder.Ascending):
                    mandates = mandates
                    .Where(collection => collection.Statuses.Any(status => status.RefStatusCode!.StatusCode == -1))
                    .OrderBy(collection => collection.Statuses.Min(status => status.StatusDate));
                    break;

                case (CollectionSortCriteria.ModificationDate, SortOrder.Ascending):
                    mandates = mandates
                        .Where(collection => collection.Statuses.Any(status => status.IsCurrent))
                        .Select(collection => new CollectionDb
                        {
                            Id = collection.Id,
                            Bank = collection.Bank,
                            BankCode = collection.BankCode,
                            BranchCode = collection.BranchCode,
                            CheckDigits = collection.CheckDigits,
                            Company = collection.Company,
                            CompanyId = collection.CompanyId,
                            JeDeclareCollection = collection.JeDeclareCollection,
                            LinkType = collection.LinkType,
                            RejectReason = collection.RejectReason,
                            AccountNumber = collection.AccountNumber,
                            Statuses = collection.Statuses.Where(status => status.IsCurrent).ToList(),
                        }).OrderBy(collection => collection.Statuses.Min(status => status.StatusDate));
                    break;

                case (CollectionSortCriteria.Status, SortOrder.Ascending):
                    mandates = mandates
                       .Where(collection => collection.Statuses.Any(status => status.IsCurrent))
                       .Select(collection => new CollectionDb
                       {
                           Id = collection.Id,
                           Bank = collection.Bank,
                           BankCode = collection.BankCode,
                           BranchCode = collection.BranchCode,
                           CheckDigits = collection.CheckDigits,
                           Company = collection.Company,
                           CompanyId = collection.CompanyId,
                           JeDeclareCollection = collection.JeDeclareCollection,
                           LinkType = collection.LinkType,
                           RejectReason = collection.RejectReason,
                           AccountNumber = collection.AccountNumber,
                           Statuses = collection.Statuses.Where(status => status.IsCurrent).ToList(),
                       }).OrderBy(collection => collection.Statuses.Min(status => status.RefStatusCode!.PulseCode));
                    break;

                case (CollectionSortCriteria.ErpId, SortOrder.Descending):
                    mandates = mandates.OrderByDescending(item => item.Company!.ErpId);
                    break;

                case (CollectionSortCriteria.Name, SortOrder.Descending):
                    mandates = mandates.OrderByDescending(item => item.Company!.Name);
                    break;

                case (CollectionSortCriteria.AccountNumber, SortOrder.Descending):
                    mandates = mandates.OrderByDescending(item => item.AccountNumber);
                    break;

                case (CollectionSortCriteria.BankName, SortOrder.Descending):
                    mandates = mandates.OrderByDescending(item => item.Bank!.BankName);
                    break;

                case (CollectionSortCriteria.CreationDate, SortOrder.Descending):
                    mandates = mandates
                    .Where(collection => collection.Statuses.Any(status => status.RefStatusCode!.StatusCode == -1))
                    .OrderByDescending(collection => collection.Statuses.Min(status => status.StatusDate));
                    break;

                case (CollectionSortCriteria.ModificationDate, SortOrder.Descending):
                    mandates = mandates
                        .Where(collection => collection.Statuses.Any(status => status.IsCurrent))
                        .Select(collection => new CollectionDb
                        {
                            Id = collection.Id,
                            Bank = collection.Bank,
                            BankCode = collection.BankCode,
                            BranchCode = collection.BranchCode,
                            CheckDigits = collection.CheckDigits,
                            Company = collection.Company,
                            CompanyId = collection.CompanyId,
                            JeDeclareCollection = collection.JeDeclareCollection,
                            LinkType = collection.LinkType,
                            RejectReason = collection.RejectReason,
                            AccountNumber = collection.AccountNumber,
                            Statuses = collection.Statuses.Where(status => status.IsCurrent).ToList(),
                        }).OrderByDescending(collection => collection.Statuses.Min(status => status.StatusDate));
                    break;

                case (CollectionSortCriteria.Status, SortOrder.Descending):
                    mandates = mandates
                       .Where(collection => collection.Statuses.Any(status => status.IsCurrent))
                       .Select(collection => new CollectionDb
                       {
                           Id = collection.Id,
                           Bank = collection.Bank,
                           BankCode = collection.BankCode,
                           BranchCode = collection.BranchCode,
                           CheckDigits = collection.CheckDigits,
                           Company = collection.Company,
                           CompanyId = collection.CompanyId,
                           JeDeclareCollection = collection.JeDeclareCollection,
                           LinkType = collection.LinkType,
                           RejectReason = collection.RejectReason,
                           AccountNumber = collection.AccountNumber,
                           Statuses = collection.Statuses.Where(status => status.IsCurrent).ToList(),
                       })
                       .OrderByDescending(collection => collection.Statuses.Min(status => status.RefStatusCode!.PulseCode));
                    break;

                default:
                    break;
            }

            var list = await mandates
                .Skip(query.Skip.HasValue ? query.Skip.Value : 0)
                .Take(query.Limit.HasValue ? query.Limit.Value : mandates.Count())
                .ToListAsync().ConfigureAwait(false);

            return (list, await mandates.CountAsync());
        }

        public async Task<List<CompanyDb?>> GetAllCompaniesByCollaboratorAsync(string email)
        {
            using var context = new MandateContext(this.options);
            var companiesForCollaborator = context.Collaborator
                .AsNoTracking()
                .Include(c => c.CompanyCollaborators)
                .ThenInclude(cc => cc.Company)
                .Where(c => c.Email == email)
                .SelectMany(c => c.CompanyCollaborators)
                .Select(cc => cc.Company);

            return await companiesForCollaborator.ToListAsync().ConfigureAwait(false);
        }

        public async Task<RefBankDb> GetRefBankByCodeAsync(string bankCode)
        {
            using var context = new MandateContext(this.options);
            var banks = context.RefBank.AsNoTracking().Where(r => r.BankCode == bankCode);
            if (!await banks.AnyAsync().ConfigureAwait(false))
            {
                throw BankCodeNotFoundException.FromId(bankCode);
            }
            else
            {
                return await banks.SingleAsync().ConfigureAwait(false);
            }
        }

        public async Task<byte[]> GetPdfTemplateByCodeAsync(string bankCode)
        {
            using var context = new MandateContext(this.options);
            var template = context.RefPdfTemplate.AsNoTracking().Where(r => r.BankCode == bankCode);
            if (!await template.AnyAsync().ConfigureAwait(false))
            {
                throw BankCodeNotFoundException.FromId(bankCode);
            }
            else
            {
                var refPdfTemplate = await template.SingleAsync().ConfigureAwait(false);
                return refPdfTemplate.PdfFile;
            }
        }

        public async Task SaveSignatoryAsync(PersonalDb personalDb)
        {
            using var context = new MandateContext(this.options);

            await context.AddAsync(personalDb);
            await context.SaveChangesAsync();
        }

        public Task<CollectionDb> GetCollectionById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<CompanyDb> GetCompanyByErpIdAsync(string erpId)
        {
            using var context = new MandateContext(this.options);
            var company = await context.Company.AsNoTracking().SingleAsync(c => c.ErpId == erpId).ConfigureAwait(false);
            if (company == null)
            {
                throw CompanyNotFoundException.FromId(erpId);
            }

            return company;
        }
    }
}
