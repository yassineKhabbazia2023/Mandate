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
            Client client = new Client()
            {
                Id = company.BankServicesProviderId!,
                RaisonSociale = company.Name!,
                Siret = new Siret()
                {
                    Siren = company.SiretNumber.Extract(0, 9),
                    Nic = company.SiretNumber.Extract(9, 5),
                },
                Responsable = new Responsable()
                {
                    Adresse = new Adresse()
                    {
                        CodePostal = company.Address?.ZipCode!,
                        CplRue = company.Address?.Complements!,
                        Pays = company.Address?.Country!,
                        Rue = company.Address?.Street!,
                        Ville = company.Address?.City!,
                    },
                    Mail = !string.IsNullOrEmpty(company.Signatory?.Email!) ? company.Signatory?.Email! : null,
                    Name = $"{company.Signatory?.Title!} {company.Signatory?.FirstName!} {company.Signatory?.LastName!.ToUpper()}",
                },
            };

            DossierClient dossier = new DossierClient() { Client = client };

            return dossier;
        }

        public static Company ToCompany(this DossierClient dossierClient)
        {
            var decomposedName = dossierClient.Client.Responsable.Name.ExtractPersonInfo();

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
                source.Id,
                company,
                rib,
                DateTime.UtcNow,
                DateTime.UtcNow,
                initStatus);

            return collection;
        }
    }
}
