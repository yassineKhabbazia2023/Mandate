// <copyright file="Responsable.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    [XmlRoot("responsable", Namespace = SerializationHelper.Namespace)]
    public class Responsable
    {
        [XmlElement("nom")]
        public string Name { get; set; } = null!;

        [XmlElement("mel")]
        public string? Mail { get; set; } = null!;

        [XmlElement("adresse")]
        public Adresse Adresse { get; set; } = null!;
    }
}
