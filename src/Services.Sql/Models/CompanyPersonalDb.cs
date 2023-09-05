// <copyright file="CompanyPersonalDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class CompanyPersonalDb
    {
        public Guid Id { get; set; }

        public Guid CompanyId { get; set; }

        public CompanyDb? Company { get; set; } = null!;

        public string? Title { get; set; } = null!;

        public string? FirstName { get; set; } = null!;

        public string? LastName { get; set; } = null!;

        public string? Email { get; set; } = null!;

        public string? Street { get; set; } = null!;

        public string? Complements { get; set; } = null!;

        public string? ZipCode { get; set; } = null!;

        public string? City { get; set; } = null!;

        public string? Country { get; set; } = null!;
    }
}
