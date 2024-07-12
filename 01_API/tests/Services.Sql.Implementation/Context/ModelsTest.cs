// <copyright file="ModelsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    [Collection("SerialExecutionPublishDb")]
    public class ModelsTest
    {
        private readonly IOptions<SqlMandateRepositoryOptions> options;

        public ModelsTest()
        {
            this.options = Options.Create(new SqlMandateRepositoryOptions()
            {
                ConnectionString = SqlServerFixture.ConnectionString,
            });
        }

        [Fact]
        public async Task Empty_Success()
        {
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);
            await context.Database.ExecuteSqlRawAsync("SELECT 1", CancellationToken.None);
            Assert.True(true);
        }

        [Fact]
        public async Task ModelToSql_Success()
        {
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);
            var refBankDb = EntityDbFactory.RefBankDb;

            await context.RefBank.AddAsync(refBankDb);
            await context.SaveChangesAsync();

            var dataTable = database.ExecuteQuery("SELECT * FROM [Mandate].[RefBank]");
            dataTable.Rows.Count.Should().Be(1);
            EntityDbFactory.FromRow<RefBankDb>(dataTable.Rows[0]).Should().BeEquivalentTo(refBankDb);
        }

        [Fact]
        public async Task SqlToModel_Success()
        {
            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);
            var refBankDb = EntityDbFactory.RefBankDb;

            var (query, parameters) = EntityDbFactory.PrepareStatement(refBankDb);
            var insertCount = await database.ExecuteNonQueryAsync(query, parameters);
            insertCount.Should().Be(1);

            var dbContent = await context.RefBank.ToArrayAsync();
            dbContent.Count().Should().Be(1);
            dbContent[0].Should().BeEquivalentTo(refBankDb);
        }
    }
}
