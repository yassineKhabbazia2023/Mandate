// <copyright file="JeDeclareExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;

    public static class JeDeclareExtensions
    {
        public static Rib ToRibClient(this MandateCreation source)
        {
            return new Rib()
            {
                Etablissement = source.Bban?.BankCode!,
                Guichet = source.Bban?.BankCode!,
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
                    Mail = company.Signatory?.Email!,
                    Name = $"{company.Signatory?.FirstName!} {company.Signatory?.LastName!}",
                },
            };

            DossierClient dossier = new DossierClient() { Client = client };

            return dossier;
        }

        public static Collection ToModel(this Releve source, Mandate.Company company, Bank bank, Collection sourceCollection)
        {
            Bban bban = new Bban(
                source.Rib?.Etablissement!,
                source.Rib?.Guichet!,
                source.Rib?.NumCompte!,
                source.Rib?.Cle!,
                source.Rib?.Id!,
                bank);

            Collection collection = new Collection(sourceCollection.Id, source.Id, company, bban, DateTime.Now, DateTime.Now, sourceCollection.Status);

            return collection;
        }
    }
}
