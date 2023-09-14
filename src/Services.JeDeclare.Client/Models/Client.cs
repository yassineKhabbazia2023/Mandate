// <copyright file="Client.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    [XmlRoot("client", Namespace = SerializationHelper.Namespace)]
    public class Client
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; } = null!;

        [XmlElement(ElementName = "siretPrincipal")]
        public Siret Siret { get; set; } = null!;

        [XmlElement(ElementName = "rs")]
        public string RaisonSocial { get; set; } = null!;

        [XmlElement(ElementName = "responsable")]
        public Responsable Responsable { get; set; } = null!;
    }
}
