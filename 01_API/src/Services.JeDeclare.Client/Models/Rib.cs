// <copyright file="Rib.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    [XmlRoot("rib", Namespace = SerializationHelper.Namespace)]
    public class Rib
    {
        [XmlElement(ElementName = "id")]
        public string? Id { get; set; }

        [XmlElement(ElementName = "libelle")]
        public string? Libelle { get; set; }

        [XmlElement(ElementName = "civiliteTitulaire")]
        public string? CiviliteTitulaire { get; set; }

        [XmlElement(ElementName = "nomTitulaire")]
        public string? NomTitulaire { get; set; }

        [XmlElement(ElementName = "prenomTitulaire")]
        public string? PrenomTitulaire { get; set; }

        [XmlElement(ElementName = "etablissement")]
        public string? Etablissement { get; set; }

        [XmlElement(ElementName = "guichet")]
        public string? Guichet { get; set; }

        [XmlElement(ElementName = "numCompte")]
        public string? NumCompte { get; set; }

        [XmlElement(ElementName = "cle")]
        public string? Cle { get; set; }
    }
}