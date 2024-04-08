// <copyright file="JeDeclareExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;

    public static class JeDeclareExtensions
    {
        public static Rib ToRibClient(this CollectionCreationCommand source)
        {
            return new Rib()
            {
                Etablissement = source.Bban?.BankCode!,
                Guichet = source.Bban?.BranchCode!,
                NumCompte = source.Bban?.AccountNumber!,
                Cle = source.Bban?.CheckDigits!,
                CiviliteTitulaire = source.Signatory?.Title!,
                NomTitulaire = source.Signatory?.LastName!,
                PrenomTitulaire = source.Signatory?.FirstName!,
            };
        }

        public static DossierClient ToDossierClient(this Company company)
        {
            Client client = new Client
            {
                Id = company.BankServicesProviderId!,
                RaisonSociale = company.Name!.Sanitize(),
                Siret = CreateSiretFromCompany(company),
                Responsable = CreateResponsableFromCompany(company),
            };

            return new DossierClient { Client = client };
        }

        public static Company ToCompany(this DossierClient dossierClient)
        {
            var decomposedName = dossierClient!.Client.Responsable.Name.ExtractPersonInfo();

            var signatory = new Signatory(
                title: decomposedName.sexe,
                firstName: decomposedName.prenom,
                lastName: decomposedName.nom,
                email: dossierClient.Client.Responsable.Mail);

            var adresse = new Address(
                street: dossierClient.Client.Responsable.Adresse.Rue,
                complements: dossierClient.Client.Responsable.Adresse.CplRue,
                zipCode: dossierClient.Client.Responsable.Adresse.CodePostal,
                city: dossierClient.Client.Responsable.Adresse.Ville,
                country: dossierClient.Client.Responsable.Adresse.Pays);

            return new Company(
                id: Guid.Empty,
                name: dossierClient.Client.RaisonSociale,
                siretNumber: dossierClient.Client.Siret.Siren + dossierClient.Client.Siret.Nic,
                erpId: null,
                bankServicesProviderId: dossierClient.Client.Id,
                signatory: signatory,
                address: adresse);
        }

        public static Releve ToReleve(this Bban rib, Signatory signatory)
        {
            var releve = new Releve()
            {
                Rib = new Rib()
                {
                    Id = rib?.BbanServicesProviderId,
                    Etablissement = rib?.BankCode,
                    Guichet = rib?.BranchCode,
                    NumCompte = rib?.AccountNumber,
                    Cle = rib?.CheckDigits,
                    CiviliteTitulaire = signatory?.Title,
                    NomTitulaire = signatory?.LastName,
                    PrenomTitulaire = signatory?.FirstName,
                },
            };

            return releve;
        }

        public static Bban ToModel(this Rib source, Bank? bank)
        {
            return new Bban(
                bankCode: source.Etablissement!,
                branchCode: source.Guichet!,
                accountNumber: source.NumCompte!,
                checkDigits: source.Cle!,
                bbanServicesProviderId: source.Id,
                bank: bank);
        }

        public static Collection ToModel(this Releve source, Company company, Bban rib, Guid collectionId, Status initStatus)
        {
            Collection collection = new Collection(
                collectionId,
                source!.Id,
                company,
                rib,
                DateTime.UtcNow,
                DateTime.UtcNow,
                initStatus);

            return collection;
        }

        public static TechnicalCollection ToModel(this Releve source)
        {
            var rib = new Bban(source.Rib?.Etablissement!, source.Rib?.Guichet!, source.Rib?.NumCompte!, source.Rib?.Cle!, source.Rib?.Id, null);
            TechnicalCollection collection = new TechnicalCollection(
                Guid.Empty,
                source.Id!,
                source.Rib?.Id!,
                new BankDetails(
                    rib.BankCode,
                    rib.BranchCode,
                    rib.AccountNumber,
                    rib.CheckDigits),
                source.Etat!);

            return collection;
        }

        private static Siret CreateSiretFromCompany(Company company)
        {
            return new Siret
            {
                Siren = company.SiretNumber.Extract(0, 9),
                Nic = company.SiretNumber.Extract(9, 5),
            };
        }

        private static Responsable CreateResponsableFromCompany(Company company)
        {
            return new Responsable
            {
                Adresse = new Adresse
                {
                    CodePostal = company.Address?.ZipCode!,
                    CplRue = company.Address?.Complements!.Sanitize() !,
                    Pays = company.Address?.Country?.Sanitize() !,
                    Rue = company.Address?.Street?.Sanitize() !,
                    Ville = company.Address?.City?.Sanitize() !,
                },
                Mail = company.Signatory?.Email,
                Name = $"{company.Signatory?.Title ?? string.Empty}" +
                $" {company.Signatory?.FirstName?.Sanitize() ?? string.Empty} " +
                $"{company.Signatory?.LastName?.Sanitize() ?? string.Empty}",
            };
        }
    }
}
