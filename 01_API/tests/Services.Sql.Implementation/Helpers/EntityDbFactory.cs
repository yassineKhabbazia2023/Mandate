// <copyright file="EntityDbFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using System.Data;
    using Microsoft.Data.SqlClient;

    public static class EntityDbFactory
    {
        public static CollectionDb CollectionDb => new()
        {
            Id = new PredictableGuid(101).NewGuid(),
            CompanyId = 102,
            BankCode = "12345",
            BranchCode = "23456",
            AccountNumber = "12345678901",
            CheckDigits = "55",
            LinkType = 7,
            RejectReason = "reason1",
        };

        public static CompanyDb CompanyDb => new()
        {
            Id = 102,
            Name = "cn1",
            SiretNumber = "12345678901234",
            ErpId = "1234567890",
        };

        public static RefBankDb RefBankDb => new()
        {
            BankCode = "12345",
            BankName = "bn1",
            BankCommercialName = "bcn",
            BankCategory = "bca",
            BankGroup = "bg",
            IsJdcScrapable = true,
            IsJdcPartner = false,
            HasReleveAgreement = false,
            HasLiasseAgreement = null,
            AllowsDemat = true,
            JdcPartnership = (JdcPartnership)2,
            EbicsCardId = null,
        };

        public static RefStatusCodeDb RefStatusCodeDb => new()
        {
            StatusCode = -1,
            CollectionStatusCode = null,
            PulseCode = 100,
            StatusNameFr = "En cours",
            StatusNameEn = "In progress",
        };

        public static StatusDb StatusDb => new()
        {
            Id = new PredictableGuid(103).NewGuid(),
            CollectionId = new PredictableGuid(101).NewGuid(),
            StatusCode = -1,
            CollectionStatusCode = null,
            IsCurrent = true,
            StatusDate = new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc),
            MandateFile = null,
            CreatedBy = "created1",
        };

        public static CollaboratorDb CollaboratorDb => new()
        {
            Id = 104,
            Email = "collab@email.com",
            FirstName = "fname",
            LastName = "lname",
        };

        public static CompanyCollaboratorDb CompanyCollaboratorDb => new()
        {
            CompanyId = 102,
            CollaboratorId = 104,
        };

        public static JeDeclareFolderDb JeDeclareFolderDb => new()
        {
            Id = new PredictableGuid(104).NewGuid(),
            CompanyId = 102,
            JdcDossierId = "folderId",
        };

        public static PersonalDb? PersonalDb => new()
        {
            Id = Guid.NewGuid(),
            CollectionId = new PredictableGuid(101).NewGuid(),
            Title = "M",
            FirstName = "Clément",
            LastName = "Prati",
            Email = "clementprati@kpmg.fr",
            Street = "36 Rue de Liège",
            Complements = "4ème étage",
            ZipCode = "75008",
            City = "Paris",
            Country = "France",
        };

        public static JeDeclareCollectionDb? JeDeclareCollectionDb => new()
        {
            Id = Guid.NewGuid(),
            CollectionId = new PredictableGuid(101).NewGuid(),
            JdcReleveId = "12346",
            JdcRibId = "12347",
        };

        public static List<StatusDb> Statuses => new()
        {
            new()
            {
                Id = Guid.NewGuid(),
                CollectionId = new PredictableGuid(101).NewGuid(),
                StatusCode = -1,
                IsCurrent = false,
                StatusDate = new DateTime(2023, 9, 28, 22, 0, 0, DateTimeKind.Utc),
                RefStatusCode = RefStatusCodeDb,
            },
        };

        public static RefPdfTemplateDb RefPdfTemplateDb => new()
        {
            BankCode = "12345",
            PdfFile = Convert.FromBase64String("dGVzdA=="),
        };

        public static T? FromRow<T>(DataRow source)
            where T : class
        {
            object? o = Activator.CreateInstance(typeof(T));
            foreach (var prop in typeof(T).GetProperties())
            {
                object? converted = null;
                if (source[prop.Name] != DBNull.Value)
                {
                    var nullType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    if (nullType.BaseType == typeof(Enum))
                    {
                        converted = Enum.ToObject(nullType, source[prop.Name]);
                    }
                    else
                    {
                        converted = Convert.ChangeType(source[prop.Name], nullType);
                    }
                }

                prop.SetValue(o, converted);
            }

            return (T?)o;
        }

        public static (string Query, SqlParameter[] Parameters) PrepareStatement(RefBankDb source)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@BankCode", SqlDbType.Char, 5) { Value = source.BankCode },
                new SqlParameter("@BankName", SqlDbType.NVarChar) { Value = source.BankName },
                new SqlParameter("@BankCommercialName", SqlDbType.NVarChar) { Value = source.BankCommercialName },
                new SqlParameter("@BankCategory", SqlDbType.NVarChar) { Value = source.BankCategory },
                new SqlParameter("@BankGroup", SqlDbType.NVarChar) { Value = source.BankGroup },
                new SqlParameter("@IsJdcScrapable", SqlDbType.Bit) { Value = source.IsJdcScrapable },
                new SqlParameter("@IsJdcPartner", SqlDbType.Bit) { Value = source.IsJdcPartner },
                new SqlParameter("@HasReleveAgreement", source.HasReleveAgreement),
                new SqlParameter("@HasLiasseAgreement", source.HasLiasseAgreement.HasValue ? source.HasLiasseAgreement.Value : DBNull.Value),
                new SqlParameter("@AllowsDemat", SqlDbType.Bit) { Value = source.AllowsDemat },
                new SqlParameter("@JdcPartnership", SqlDbType.TinyInt) { Value = (byte)source.JdcPartnership },
            };
            return (@"INSERT INTO [Mandate].[RefBank] 
            ([BankCode],[BankName],[BankCommercialName],[BankCategory],[BankGroup],[IsJdcScrapable],[IsJdcPartner],[HasReleveAgreement],[HasLiasseAgreement],[AllowsDemat],[JdcPartnership])
            VALUES
            (@BankCode,@BankName,@BankCommercialName,@BankCategory,@BankGroup,@IsJdcScrapable,@IsJdcPartner,@HasReleveAgreement,@HasLiasseAgreement,@AllowsDemat,@JdcPartnership)", parameters.ToArray());
        }
    }
}