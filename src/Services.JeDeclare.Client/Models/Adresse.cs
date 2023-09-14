// <copyright file="Adresse.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    [XmlRoot("adresse", Namespace = SerializationHelper.Namespace)]
    public class Adresse
    {
        [XmlElement("rue")]
        public string Rue { get; set; } = null!;

        [XmlElement("cplRue")]
        public string CplRue { get; set; } = null!;

        [XmlElement("codePostal")]
        public string CodePostal { get; set; } = null!;

        [XmlElement("ville")]
        public string Ville { get; set; } = null!;

        [XmlElement("pays")]
        public string Pays { get; set; } = null!;
    }
}
