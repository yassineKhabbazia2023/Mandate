// <copyright file="Siret.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    [XmlRoot("siretPrincipal", Namespace = SerializationHelper.Namespace)]
    public class Siret
    {
        [XmlElement(ElementName = "siren")]
        public string Siren { get; set; } = null!;

        [XmlElement(ElementName = "nic")]
        public string Nic { get; set; } = null!;
    }
}
