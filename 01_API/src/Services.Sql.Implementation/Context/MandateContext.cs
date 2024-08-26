// <copyright file="MandateContext.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class MandateContext : DbContext
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

        public DbSet<CollaboratorDb> Collaborator { get; set; } = null!;

        public DbSet<CollectionDb> Collection { get; set; } = null!;

        public DbSet<CompanyCollaboratorDb> CompanyCollaborator { get; set; } = null!;

        public DbSet<CompanyDb> Company { get; set; } = null!;

        public DbSet<PersonalDb> Personal { get; set; } = null!;

        public DbSet<JeDeclareCollectionDb> JeDeclareCollection { get; set; } = null!;

        public DbSet<JeDeclareFolderDb> JeDeclareFolder { get; set; } = null!;

        public DbSet<RefBankDb> RefBank { get; set; } = null!;

        public DbSet<RefPdfTemplateDb> RefPdfTemplate { get; set; } = null!;

        public DbSet<RefStatusCodeDb> RefStatusCode { get; set; } = null!;

        public DbSet<StatusDb> Status { get; set; } = null!;

        public DbSet<MandateLogDb> MandateLog { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Mandate");

            modelBuilder.Entity<CollaboratorDb>().HasKey(c => c.Id);
            modelBuilder.Entity<CollaboratorDb>().Property(c => c.Email).HasMaxLength(255).IsRequired(true);
            modelBuilder.Entity<CollaboratorDb>().Property(c => c.FirstName).HasMaxLength(255).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<CollaboratorDb>().Property(c => c.LastName).HasMaxLength(255).IsUnicode(true).IsRequired(false);

            modelBuilder.Entity<CollectionDb>().HasKey(c => c.Id);
            modelBuilder.Entity<CollectionDb>().HasOne(s => s.Company).WithMany(c => c.Collections).HasForeignKey(s => s.CompanyId);
            modelBuilder.Entity<CollectionDb>().HasOne(c => c.Personal)
                .WithOne(p => p.Collection)
                .HasForeignKey<PersonalDb>(p => p.CollectionId)
                .IsRequired(false);
            modelBuilder.Entity<CollectionDb>().Property(cp => cp.BankCode).IsFixedLength(true).HasMaxLength(5).IsRequired(true);
            modelBuilder.Entity<CollectionDb>().Property(cp => cp.BranchCode).IsFixedLength(true).HasMaxLength(5).IsRequired(true);
            modelBuilder.Entity<CollectionDb>().Property(cp => cp.AccountNumber).IsFixedLength(true).HasMaxLength(11).IsRequired(true);
            modelBuilder.Entity<CollectionDb>().Property(cp => cp.CheckDigits).IsFixedLength(true).HasMaxLength(2).IsRequired(true);
            modelBuilder.Entity<CollectionDb>().Property(cp => cp.RejectReason).HasMaxLength(100).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<CollectionDb>().Property(cp => cp.LinkType).IsRequired(false);
            modelBuilder.Entity<CollectionDb>().HasOne(c => c.JeDeclareCollection)
                .WithOne(jdc => jdc.Collection)
                .HasForeignKey<JeDeclareCollectionDb>(jdc => jdc.CollectionId)
                .IsRequired(false);
            modelBuilder.Entity<CollectionDb>().HasOne(c => c.Bank).WithMany().HasForeignKey(c => c.BankCode);

            // Configure the composite primary key for the CompanyCollaborator table
            modelBuilder.Entity<CompanyCollaboratorDb>().Property(cc => cc.CompanyId).IsRequired(true);
            modelBuilder.Entity<CompanyCollaboratorDb>().Property(cc => cc.CollaboratorId).IsRequired(true);
            modelBuilder.Entity<CompanyCollaboratorDb>()
                .HasKey(cc => new { cc.CompanyId, cc.CollaboratorId });

            // Configure the many-to-many relationship
            modelBuilder.Entity<CompanyCollaboratorDb>()
                .HasOne(cc => cc.Company)
                .WithMany(c => c.CompanyCollaborators)
                .HasForeignKey(cc => cc.CompanyId);
            modelBuilder.Entity<CompanyCollaboratorDb>()
                .HasOne(cc => cc.Collaborator)
                .WithMany(c => c.CompanyCollaborators)
                .HasForeignKey(cc => cc.CollaboratorId);

            modelBuilder.Entity<CompanyDb>().HasKey(c => c.Id);
            modelBuilder.Entity<CompanyDb>().HasOne(c => c.Personal)
                .WithOne(cp => cp.Company)
                .HasForeignKey<PersonalDb>(cp => cp.CompanyId)
                .IsRequired(false);
            modelBuilder.Entity<CompanyDb>().HasOne(c => c.JeDeclareFolder)
                .WithOne(jdf => jdf.Company)
                .HasForeignKey<JeDeclareFolderDb>(jdf => jdf.CompanyId);
            modelBuilder.Entity<CompanyDb>().Property(c => c.Name).HasMaxLength(255).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<CompanyDb>().Property(c => c.SiretNumber).HasMaxLength(150).IsRequired(false);
            modelBuilder.Entity<CompanyDb>().Property(c => c.ErpId).HasMaxLength(50).IsRequired(false);

            modelBuilder.Entity<PersonalDb>().HasKey(cp => cp.Id);
            modelBuilder.Entity<PersonalDb>().Property(cp => cp.Title).HasMaxLength(10).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<PersonalDb>().Property(cp => cp.FirstName).HasMaxLength(100).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<PersonalDb>().Property(cp => cp.LastName).HasMaxLength(100).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<PersonalDb>().Property(cp => cp.Email).HasMaxLength(100).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<PersonalDb>().Property(cp => cp.Street).HasMaxLength(100).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<PersonalDb>().Property(cp => cp.Complements).HasMaxLength(100).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<PersonalDb>().Property(cp => cp.ZipCode).HasMaxLength(100).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<PersonalDb>().Property(cp => cp.City).HasMaxLength(100).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<PersonalDb>().Property(cp => cp.Country).HasMaxLength(100).IsUnicode(true).IsRequired(false);

            modelBuilder.Entity<JeDeclareCollectionDb>().HasKey(jdc => jdc.Id);
            modelBuilder.Entity<JeDeclareCollectionDb>().Property(cp => cp.JdcReleveId).HasMaxLength(50).IsRequired(false);
            modelBuilder.Entity<JeDeclareCollectionDb>().Property(cp => cp.JdcRibId).HasMaxLength(50).IsRequired(false);

            modelBuilder.Entity<JeDeclareFolderDb>().HasKey(cp => cp.Id);
            modelBuilder.Entity<JeDeclareFolderDb>().Property(cp => cp.JdcDossierId).HasMaxLength(50).IsRequired(false);

            modelBuilder.Entity<RefBankDb>().HasKey(b => b.BankCode);
            modelBuilder.Entity<RefBankDb>().Property(s => s.BankCode).IsFixedLength(true).HasMaxLength(5).IsRequired(true);
            modelBuilder.Entity<RefBankDb>().Property(s => s.BankName).HasMaxLength(250).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<RefBankDb>().Property(s => s.BankCommercialName).HasMaxLength(250).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<RefBankDb>().Property(s => s.BankCategory).HasMaxLength(250).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<RefBankDb>().Property(s => s.BankGroup).HasMaxLength(250).IsUnicode(true).IsRequired(false);
            modelBuilder.Entity<RefBankDb>().Property(s => s.IsJdcScrapable).IsRequired(true).HasDefaultValue(false);
            modelBuilder.Entity<RefBankDb>().Property(s => s.IsJdcPartner).IsRequired(true).HasDefaultValue(false);
            modelBuilder.Entity<RefBankDb>().Property(s => s.HasReleveAgreement).IsRequired(false);
            modelBuilder.Entity<RefBankDb>().Property(s => s.HasLiasseAgreement).IsRequired(false);
            modelBuilder.Entity<RefBankDb>().Property(s => s.AllowsDemat).IsRequired(false);
            modelBuilder.Entity<RefBankDb>().Property(s => s.JdcPartnership).IsRequired(true);
            modelBuilder.Entity<RefBankDb>().Property(s => s.EbicsCardId).HasMaxLength(50).IsRequired(false);

            modelBuilder.Entity<RefPdfTemplateDb>().HasKey(s => s.BankCode);
            modelBuilder.Entity<RefPdfTemplateDb>().Property(s => s.BankCode).IsFixedLength(true).HasMaxLength(5).IsRequired(true);
            modelBuilder.Entity<RefPdfTemplateDb>().Property(s => s.PdfFile).IsRequired(true);

            modelBuilder.Entity<RefStatusCodeDb>().HasKey(s => s.StatusCode);
            modelBuilder.Entity<RefStatusCodeDb>().Property(s => s.StatusCode).IsRequired(true).ValueGeneratedNever();
            modelBuilder.Entity<RefStatusCodeDb>().Property(s => s.CollectionStatusCode).IsRequired(false);
            modelBuilder.Entity<RefStatusCodeDb>().Property(s => s.PulseCode).IsRequired(true);
            modelBuilder.Entity<RefStatusCodeDb>().Property(s => s.StatusNameFr).HasMaxLength(100).IsUnicode(true).IsRequired(true);
            modelBuilder.Entity<RefStatusCodeDb>().Property(s => s.StatusNameEn).HasMaxLength(100).IsUnicode(true).IsRequired(true);

            modelBuilder.Entity<StatusDb>().HasKey(s => s.Id);
            modelBuilder.Entity<StatusDb>().HasOne(s => s.Collection).WithMany(c => c.Statuses).HasForeignKey(s => s.CollectionId);
            modelBuilder.Entity<StatusDb>().HasOne(s => s.RefStatusCode).WithMany().HasForeignKey(s => s.StatusCode);
            modelBuilder.Entity<StatusDb>().Property(cp => cp.StatusCode).IsRequired(true);
            modelBuilder.Entity<StatusDb>().Property(cp => cp.CollectionStatusCode).IsRequired(false);
            modelBuilder.Entity<StatusDb>().Property(cp => cp.IsCurrent).IsRequired(true);
            modelBuilder.Entity<StatusDb>().Property(cp => cp.StatusDate).IsRequired(false);
            modelBuilder.Entity<StatusDb>().Property(cp => cp.MandateFile).IsRequired(false);
            modelBuilder.Entity<StatusDb>().Property(cp => cp.CreatedBy).HasMaxLength(100).IsUnicode(true).IsRequired(false);

            modelBuilder.Entity<MandateLogDb>().HasKey(c => c.Id);
            modelBuilder.Entity<MandateLogDb>().Property(c => c.ErpId).HasMaxLength(50).IsRequired(true);
            modelBuilder.Entity<MandateLogDb>().Property(c => c.SiretNumber).IsFixedLength(true).HasMaxLength(14).IsRequired(true);
            modelBuilder.Entity<MandateLogDb>().Property(cp => cp.BankCode).IsFixedLength(true).HasMaxLength(5).IsRequired(true);
            modelBuilder.Entity<MandateLogDb>().Property(cp => cp.BranchCode).IsFixedLength(true).HasMaxLength(5).IsRequired(true);
            modelBuilder.Entity<MandateLogDb>().Property(cp => cp.AccountNumber).IsFixedLength(true).HasMaxLength(11).IsRequired(true);
            modelBuilder.Entity<MandateLogDb>().Property(cp => cp.CheckDigits).IsFixedLength(true).HasMaxLength(2).IsRequired(true);
            modelBuilder.Entity<MandateLogDb>().Property(cp => cp.JdcDossierId).HasMaxLength(50).IsRequired(true);
            modelBuilder.Entity<MandateLogDb>().Property(cp => cp.JdcReleveId).HasMaxLength(50).IsRequired(true);
            modelBuilder.Entity<MandateLogDb>().Property(cp => cp.JdcRibId).HasMaxLength(50).IsRequired(true);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer(this.options.Value.ConnectionString, sqlOptions => { sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null); });
        }
    }
}
