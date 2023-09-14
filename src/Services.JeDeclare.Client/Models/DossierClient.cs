// <copyright file="DossierClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    [XmlRoot("dossierClient", Namespace = SerializationHelper.Namespace)]
    public class DossierClient
    {
        [XmlElement(ElementName = "client")]
        public Client Client { get; set; } = null!;

        [XmlElement(ElementName = "exploitationDonnees")]
        public bool ExploitationDonnees { get; set; }
    }
}
