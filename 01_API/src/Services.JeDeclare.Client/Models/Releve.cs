// <copyright file="Releve.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    [XmlRoot("releve", Namespace = SerializationHelper.Namespace)]
    public class Releve
    {
        [XmlElement(ElementName = "id")]
        public string? Id { get; set; }

        [XmlElement(ElementName = "etat")]
        public string? Etat { get; set; }

        [XmlElement(ElementName = "etatCollecte")]
        public string? EtatCollecte { get; set; }

        [XmlElement(ElementName = "typeLiaison")]
        public string? TypeLiaison { get; set; }

        [XmlElement(ElementName = "causeRejet")]
        public string? CauseRejet { get; set; }

        [XmlElement(ElementName = "destinataire")]
        public Destinataire? Destinataire { get; set; }

        [XmlElement(ElementName = "rib")]
        public Rib? Rib { get; set; }

        [XmlElement(ElementName = "carte")]
        public Carte? Card { get; set; }

        [XmlElement(ElementName = "periodicite")]
        public Periodicite? Periodicite { get; set; }

        [XmlElement(ElementName = "dateReprise")]
        public string? DateReprise { get; set; }
    }
}