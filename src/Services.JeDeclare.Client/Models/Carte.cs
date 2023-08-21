// <copyright file="Carte.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    [XmlType(Namespace = SerializationHelper.Namespace)]
    public class Carte
    {
        [XmlElement(ElementName = "id")]
        public string? Id { get; set; }

        [XmlElement(ElementName = "statut")]
        public string? Statut { get; set; }

        [XmlElement(ElementName = "codeBanque")]
        public string? CodeBanque { get; set; }

        [XmlElement(ElementName = "nomConfig")]
        public string? NomConfig { get; set; }

        [XmlElement(ElementName = "userId")]
        public string? UserId { get; set; }

        [XmlElement(ElementName = "partnerId")]
        public string? PartnerId { get; set; }

        [XmlElement(ElementName = "emailResponsable")]
        public string? EmailResponsable { get; set; }

        [XmlElement(ElementName = "fileFormat")]
        public string? FileFormat { get; set; }

        [XmlElement(ElementName = "carteEBICs")]
        public string? CarteEBICs { get; set; }
    }
}