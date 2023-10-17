// <copyright file="JeDeclareFolder.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class JeDeclareFolder
    {
        public JeDeclareFolder(Guid id, string? jdcDossierId, Company? company)
        {
            this.Id = id;
            this.JdcDossierId = jdcDossierId;
            this.Company = company;
        }

        public Guid Id { get; }

        public string? JdcDossierId { get; }

        public Company? Company { get; }
    }
}
