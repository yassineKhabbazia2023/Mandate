// <copyright file="ListeReleves.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    [XmlRoot("listeReleves", Namespace = SerializationHelper.Namespace)]
    public class ListeReleves
    {
        [XmlElement(ElementName = "releve")]
        public Releve[]? Releve { get; set; }
    }
}
