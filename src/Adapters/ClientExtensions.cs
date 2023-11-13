// <copyright file="ClientExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    public static class ClientExtensions
    {
        public static Client.BankDetail ToBankDetail(this Bank source)
        {
            var bankJdcDetail = new Client.BankJdcDetail(source.JdcAgreement.JdcPartnership.ToString("G"));
            return new Client.BankDetail(source.Code, source.Name, bankJdcDetail);
        }

        public static Client.CollectionSummary ToCollectionSummary(this Collection source)
        {
            return new Client.CollectionSummary(
                    id: source.Id,
                    erpId: source.Company?.ErpId!,
                    companyName: source.Company?.Name!,
                    bankName: source.Bban?.Bank?.Name!,
                    accountNumber: source.Bban?.AccountNumber!,
                    creationDate: source.CreationDate,
                    modificationDate: source.ModificationDate,
                    statusCode: (int)source.Status.StatusCode);
        }

        public static Bban ToModel(this Client.Bban source)
        {
            return new Bban(source.BankCode, source.BranchCode, source.AccountNumber, source.CheckDigits, null, null);
        }

        public static CollectionQueryDto ToModel(this Client.CollectionQuery source, Guid? collaboratorId)
        {
            SortOrder sortOrder;
            if (!Enum.TryParse(source.SortOrder, out sortOrder))
            {
                var allowedSortOrder = string.Join(", ", Enum.GetNames(typeof(SortOrder)));
                throw new InvalidCastException($"Invalid SortOrder. Allowed values are [ {allowedSortOrder}]");
            }

            CollectionSortCriteria sortCriteria;
            if (!Enum.TryParse(source.SortCriteria, out sortCriteria))
            {
                var allowedSortCriteria = string.Join(", ", Enum.GetNames(typeof(CollectionSortCriteria)));
                throw new InvalidCastException($"Invalid SortCriteria. Allowed values are [ {allowedSortCriteria}]");
            }

            return new CollectionQueryDto(source.SearchTerm, source.CreationDateStart, source.CreationDateEnd, source.ModificationDateStart, source.ModificationDateEnd, source.StatusCodes, source.Limit, source.Skip, sortOrder, sortCriteria, collaboratorId.Value);
        }
    }
}
