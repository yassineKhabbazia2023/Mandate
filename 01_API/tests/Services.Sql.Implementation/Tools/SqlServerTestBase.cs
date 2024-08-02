using Microsoft.Extensions.Options;

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests.Tools
{
    [Collection("SqlServerXunitCollection")]
    public abstract class SqlServerTestBase
    {
        protected readonly IOptions<SqlMandateRepositoryOptions> _options;
        protected readonly SqlServerFixture _sqlServerFixture;

        protected SqlServerTestBase(SqlServerFixture sqlServerFixture)
        {
            _sqlServerFixture = sqlServerFixture;
            _options = Options.Create(new SqlMandateRepositoryOptions()
            {
                ConnectionString = _sqlServerFixture.ConnectionString,
            });

            sqlServerFixture.ClearTables();
        }
    }
}
