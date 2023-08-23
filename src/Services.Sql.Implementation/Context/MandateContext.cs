// <copyright file="MandateContext.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    internal class MandateContext : DbContext
    {
        private readonly IOptions<SqlMandateRepositoryOptions> options;

        public MandateContext(IOptions<SqlMandateRepositoryOptions> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Value.Validate();
            this.options = options;
        }

        internal DbSet<CollectionDb> Collection { get; set; } = null!;

        internal DbSet<CompanyDb> Company { get; set; } = null!;

        internal DbSet<CompanyPersonalDb> CompanyPersonal { get; set; } = null!;

        internal DbSet<JeDeclareCollectionDb> JeDeclareCollection { get; set; } = null!;

        internal DbSet<JeDeclareFolderDb> JeDeclareFolder { get; set; } = null!;

        internal DbSet<StatusDb> Status { get; set; } = null!;

        internal DbSet<RefBankDb> RefBank { get; set; } = null!;

        internal DbSet<RefStatusCodeDb> RefStatusCode { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Mandate");

            modelBuilder.Entity<CompanyDb>().HasKey(c => c.Id);
            // TODO

            modelBuilder.Entity<StatusDb>().HasKey(s => s.Id);
            modelBuilder.Entity<StatusDb>().HasOne(s => s.Collection).WithMany(c => c.Statuses).HasForeignKey(s => s.CollectionId);
            modelBuilder.Entity<StatusDb>().HasOne(s => s.RefStatusCode).WithMany().HasForeignKey(s => s.StatusCode);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(this.options.Value.ConnectionString, sqlOptions => { sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), null); });
        }
    }
}
