// <copyright file="SqlMandateRepositoryOptions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation
{
    public class SqlMandateRepositoryOptions
    {
        public string? ConnectionString { get; set; } = null!;

        internal void Validate()
        {
            if (this.ConnectionString is null)
            {
                throw new InvalidOperationException($"Instance of {nameof(SqlMandateRepositoryOptions)} is invalid, {nameof(SqlMandateRepositoryOptions.ConnectionString)} is null");
            }
        }
    }
}
