// <copyright file="ClientExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    public static class ClientExtensions
    {
        public static Client.BankDetail ToBankDetail(this Bank source)
        {
            var bankJdcDetail = new Client.BankJdcDetail(source.JdcAgreement.JdcPartnership.ToString("G") !);
            return new Client.BankDetail(source.Code!, source.Name, bankJdcDetail!);
        }

        public static Client.CollectionSummary ToCollectionSummary(this Collection source)
        {
            var collectionBankInfo = new Client.CollectionBankInfo(
                bankName: source!.Bban?.Bank?.Name!,
                accountNumber: source!.Bban?.AccountNumber!,
                jdcPartnership: (int)source!.Bban?.Bank?.JdcAgreement.JdcPartnership!);

            return new Client.CollectionSummary(
                    id: source.Id,
                    erpId: source.Company?.ErpId!,
                    companyName: source.Company?.Name!,
                    collectionBankInfo: collectionBankInfo,
                    creationDate: source.CreationDate,
                    modificationDate: source.ModificationDate,
                    statusCode: (int)source.Status.StatusCode!);
        }

        public static Client.TechnicalCollectionSummary ToTechnicalCollectionSummary(this Collection source)
        {
            return new Client.TechnicalCollectionSummary(
                    id: source.Id,
                    folderId: source.Company!.BankServicesProviderId!,
                    ribId: source.Bban?.BbanServicesProviderId!,
                    bankDetails: new Client.BankDetails(source.Bban?.BankCode!, source.Bban?.BranchCode!, source.Bban?.AccountNumber!, source.Bban?.CheckDigits!),
                    statusCode: (int)source.Status.StatusCode!);
        }

        public static Client.Counters ToCountersDetail(this Counters source)
        {
            return new Client.Counters(source.All!, source.Status10!, source.Status20!, source.Status30!, source.Status40!, source.Status50!);
        }

        public static Client.PagedMandate ToPageMandateDetails(this PagedMandate source)
        {
            return new Client.PagedMandate(
                source.Counters.ToCountersDetail(),
                source.Data.Select(r => r.ToCollectionSummary()).ToList());
        }

        public static Client.PagedTechnicalMandate ToPageTechnicalMandateDetails(this PagedTechnicalMandate source)
        {
            return new Client.PagedTechnicalMandate(
                source!.Data.Select(r => r.ToTechnicalCollectionSummary()).ToList());
        }

        public static string ToRibString(this Bban bban)
        {
            if (bban == null)
            {
                return string.Empty;
            }

            return string.Join("-", new List<string> { bban!.BankCode, bban!.BranchCode, bban!.AccountNumber, bban!.CheckDigits });
        }

        public static TechnicalCollection ToModel(this Client.TechnicalCollectionSummary source)
        {
            return new TechnicalCollection(source!.Id, source!.FolderId, source!.RibId, new BankDetails(source!.BankDetails!.BankCode, source!.BankDetails!.BranchCode, source!.BankDetails!.AccountNumber, source!.BankDetails!.CheckDigits), source!.StatusCode.ToString());
        }

        public static Address ToModel(this Client.Address source)
        {
            return new Address(source.Street, source.AddressComplement, source.ZipCode, source.City, source.Country);
        }

        public static Bban ToModel(this Client.Bban source)
        {
            return new Bban(source.BankCode, source.BranchCode, source.AccountNumber, source.CheckDigits, null, null);
        }

        public static CollectionCreationCommand ToModel(this Client.CollectionCreationCommand source)
        {
            return new CollectionCreationCommand(source.ErpId, source.Signatory.ToModel(), source.Address.ToModel(), source.Bban.ToModel());
        }

        public static CollectionQueryDto ToModel(this Client.CollectionQuery source)
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

            return new CollectionQueryDto(source.SearchTerm, source.CreationDateStart, source.CreationDateEnd, source.ModificationDateStart, source.ModificationDateEnd, source.StatusCodes, source.Limit, source.Skip, sortOrder, sortCriteria, source.CollaboratorEmail);
        }

        public static Signatory ToModel(this Client.Signatory source)
        {
            return new Signatory(source.Title, source.FirstName, source.LastName, source.Email);
        }
    }
}
