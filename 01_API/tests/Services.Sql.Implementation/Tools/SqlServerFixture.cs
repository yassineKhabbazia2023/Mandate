using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;
using System.Data;
using System.Diagnostics;

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests;
public class SqlServerFixture : IAsyncLifetime
{
    private const string Instance = "cst-unit-tests";
    private const string DatabaseName = "Mandate.Sql.Database";
    private const string DacName = $"{DatabaseName}.dacpac";
    public SqlConnection _connection;

    public string ConnectionString { get; private set; }

    private static void SqlOperation(string arguments)
    {
        var parameters = new ProcessStartInfo()
        {
            CreateNoWindow = true,
            Arguments = arguments,
            FileName = "SqlLocalDB.exe",
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        using var createDb = Process.Start(parameters);
        createDb!.WaitForExit();

        if (createDb.ExitCode != 0)
        {
            throw new Exception($"Couldn't execute SqlLocalDb with arguments '{arguments}'");
        }
    }

    public Task InitializeAsync()
    {
        SqlOperation($"create {Instance} -s");
        var csBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = Environment.GetEnvironmentVariable("UNITTESTS_SQLSERVER_DATASOURCE") ?? $"(localdb)\\{Instance}",
            InitialCatalog = DatabaseName,
            IntegratedSecurity = true,
        };

        ConnectionString = csBuilder.ConnectionString;

        var dacDeployOptions = new DacDeployOptions
        {
            CreateNewDatabase = true,
        };

        using var dacPackage = DacPackage.Load(DacName);
        var dacServices = new DacServices(ConnectionString);
        dacServices.Deploy(dacPackage, DatabaseName, true, dacDeployOptions, CancellationToken.None);
        _connection = new SqlConnection(ConnectionString);
        _connection.Open();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_connection != null)
        {
            await DropDatabase();
            _connection.Dispose();
        }
    }

    internal async Task DropDatabase()
    {
        await ExecuteNonQueryAsync($"ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE").ConfigureAwait(false);
        await ExecuteNonQueryAsync("USE master").ConfigureAwait(false);
        await ExecuteNonQueryAsync($"DROP DATABASE [{DatabaseName}]").ConfigureAwait(false);
    }

    public async Task<int> ExecuteNonQueryAsync(string sqlCommand, params SqlParameter[] arguments)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = sqlCommand;
        command.Parameters.AddRange(arguments);
        return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public void ClearTables()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM Onboarding.SepaMandates
            DELETE FROM Onboarding.PaymentPreferences
            DELETE FROM Mandate.JeDeclareFolder
            DELETE FROM Mandate.JeDeclareCollection
            DELETE FROM Mandate.CompanyCollaborator
            DELETE FROM Mandate.Status
            DELETE FROM Mandate.Personal
            DELETE FROM Mandate.Collection
            DELETE FROM Mandate.RefBank
            DELETE FROM Mandate.RefPdfTemplate
            DELETE FROM Mandate.RefStatusCode
            DELETE FROM Mandate.Company
            DELETE FROM Mandate.Collaborator
";
        command.ExecuteNonQuery();
    }

    public DataTable ExecuteQuery(string query)
    {
        using var adapter = new SqlDataAdapter(query, _connection);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);
        return dataTable;
    }
}
