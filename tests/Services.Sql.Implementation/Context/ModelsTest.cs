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
        [Fact]
        public async Task Empty_Success()
        {
            using var database = SqlServerFixture.CreateDatabase();

            var options = Options.Create(new SqlMandateRepositoryOptions()
            {
                ConnectionString = database.ConnectionString,
            });

            using var context = new MandateContext(options);
            await context.Database.ExecuteSqlRawAsync("SELECT 1", CancellationToken.None);
            Assert.True(true);
        }

        [Fact]
        public async Task Models_Success()
        {
            using var database = SqlServerFixture.CreateDatabase();

            var options = Options.Create(new SqlMandateRepositoryOptions()
            {
                ConnectionString = database.ConnectionString,
            });

            using var context = new MandateContext(options);
            var refBankDb = EntityDbFactory.RefBankDb;

            await context.RefBank.AddAsync(refBankDb);
            await context.SaveChangesAsync();

            var dataTable = database.ExecuteQuery("SELECT * FROM [Mandate].[RefBank]");
            dataTable.Rows.Count.Should().Be(1);
            EntityDbFactory.FromRow(dataTable.Rows[0]).Should().BeEquivalentTo(refBankDb);
        }
    }
}
