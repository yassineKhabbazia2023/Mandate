// <copyright file="Periodicite.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    [XmlType(Namespace = SerializationHelper.Namespace)]
    public class Periodicite
    {
        [XmlElement(ElementName = "id")]
        public string? Id { get; set; }
    }
}