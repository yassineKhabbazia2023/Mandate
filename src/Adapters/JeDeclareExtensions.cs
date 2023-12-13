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

        // déplacer vers infra jeDeclare
        public static Releve ConstructReleve(this Rib source, string? bankCode, string? ebicsCardId, string historyDateEnabledBanks)
        {
            Releve releve = new Releve()
            {
                Rib = source,
                Etat = "2",
                Periodicite = new Periodicite
                {
                    Id = "1",
                },
            };

            if (!string.IsNullOrWhiteSpace(ebicsCardId))
            {
                releve.TypeLiaison = "2";
                releve.Card = new Carte
                {
                    Id = ebicsCardId,
                };
            }

            if (historyDateEnabledBanks.Split(';').Contains(bankCode))
            {
                releve.DateReprise = $"{DateTime.Now.Year}-01-01";
            }

            return releve;
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
                        CplRue = company.Address?.Street!,
                        Pays = company.Address?.Country!,
                        Rue = company.Address?.Street!,
                        Ville = company.Address?.City!,
                    },
                    Mail = company.Signatory?.Email !,
                    Name = $"{company.Signatory?.FirstName!} {company.Signatory?.LastName!}",
                },
            };

            DossierClient dossier = new DossierClient() { Client = client };

            return dossier;
        }

        public static Company ToCompany(this DossierClient dossierClient)
        {
            var signatory = new Signatory(
                title: "",  // TO DO
                firstName: dossierClient.Client.Responsable.Name, // TO DO
                lastName: dossierClient.Client.Responsable.Name, // TO DO
                email: dossierClient.Client.Responsable.Mail);

            var adresse = new Address(
                street: dossierClient.Client.Responsable.Adresse.Rue,
                complements: dossierClient.Client.Responsable.Adresse.CplRue,
                zipCode: dossierClient.Client.Responsable.Adresse.CodePostal,
                city: dossierClient.Client.Responsable.Adresse.Ville,
                country: dossierClient.Client.Responsable.Adresse.Pays);

            return new Company(
                id: Guid.Empty, // TO DO
                name: dossierClient.Client.RaisonSociale,
                siretNumber: dossierClient.Client.Siret.Siren + dossierClient.Client.Siret.Nic,
                erpId: "", // TO DO
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
                    Etablissement = rib.BankCode,
                    Guichet = rib.BranchCode,
                    Cle = rib.CheckDigits,
                    CiviliteTitulaire = signatory.Title,
                    NomTitulaire = signatory.LastName,
                    PrenomTitulaire = signatory.FirstName,
                }, 
            };

            return releve;
        }

        //public static Collection ToCollection(this Releve releve)
        //{
        //    var collection = new Collection(
        //        id: Guid.Empty,
        //        collectionServicesProviderId: "",
        //        company: null,
        //        bban: null,
        //        creationDate: releve.Periodicite.)
        //}
    }
}
