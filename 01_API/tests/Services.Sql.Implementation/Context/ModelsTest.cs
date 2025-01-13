// <copyright file="ModelsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests.Tools;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class ModelsTest: SqlServerTestBase
    {
        public ModelsTest(SqlServerFixture sqlServerFixture) 
            : base(sqlServerFixture)
        {
        }

        private DbContextOptions<MandateContext> CreateContextOptions()
        {
            return new DbContextOptionsBuilder<MandateContext>()
                .UseSqlServer(_sqlServerFixture.ConnectionString)
                .Options;
        }

        [Fact]
        public async Task Empty_Success()
        {
            var options = CreateContextOptions();
            using var context = new MandateContext(options);

            await context.Database.ExecuteSqlRawAsync("SELECT 1", CancellationToken.None);
            Assert.True(true);
        }

        [Fact]
        public async Task ModelToSql_Success()
        {
            var options = CreateContextOptions();
            using var context = new MandateContext(options);

            var refBankDb = EntityDbFactory.RefBankDb;

            await context.RefBank.AddAsync(refBankDb);
            await context.SaveChangesAsync();

            var dataTable = _sqlServerFixture.ExecuteQuery("SELECT * FROM [Mandate].[RefBank]");
            dataTable.Rows.Count.Should().Be(1);
            EntityDbFactory.FromRow<RefBankDb>(dataTable.Rows[0]).Should().BeEquivalentTo(refBankDb);
        }

        [Fact]
        public async Task SqlToModel_Success()
        {
            var options = CreateContextOptions();
            using var context = new MandateContext(options);

            var refBankDb = EntityDbFactory.RefBankDb;

            var (query, parameters) = EntityDbFactory.PrepareStatement(refBankDb);
            var insertCount = await _sqlServerFixture.ExecuteNonQueryAsync(query, parameters);
            insertCount.Should().Be(1);

            var dbContent = await context.RefBank.ToArrayAsync();
            dbContent.Count().Should().Be(1);
            dbContent[0].Should().BeEquivalentTo(refBankDb);
        }
    }
}
