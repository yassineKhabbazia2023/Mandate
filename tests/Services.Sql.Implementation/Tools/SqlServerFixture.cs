// <copyright file="SqlServerFixture.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using System.Diagnostics;
    using Microsoft.Data.SqlClient;
    using Microsoft.SqlServer.Dac;

    internal class SqlServerFixture : IDisposable
    {
        private const string Instance = "cst-unit-tests";
        private const string DatabaseName = "Mandate.Sql.Database";
        private const string DacName = $"{DatabaseName}.dacpac";
        private static string connectionString = string.Empty;
        private bool disposedValue = false;

        public SqlServerFixture()
        {
            SqlOperation($"create {Instance} -s");
            var csBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = Environment.GetEnvironmentVariable("UNITTESTS_SQLSERVER_DATASOURCE") ?? $"(localdb)\\{Instance}",
                InitialCatalog = DatabaseName,
                IntegratedSecurity = true,
            };

            connectionString = csBuilder.ConnectionString;
        }

        ~SqlServerFixture()
        {
            this.Dispose(disposing: false);
        }

        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal static SqlServerDatabase CreateDatabase()
        {
            var dacDeployOptions = new DacDeployOptions()
            {
                CreateNewDatabase = true,
            };

            using var dacPackage = DacPackage.Load(DacName);
            var dacServices = new DacServices(connectionString);
            dacServices.Deploy(dacPackage, DatabaseName, true, dacDeployOptions, CancellationToken.None);
            var database = new SqlServerDatabase(connectionString);
            Connect(database);
            return database;
        }

        internal static void DropDatabase(SqlServerDatabase database)
        {
            database.ExecuteNonQuery("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", DatabaseName);
            database.ExecuteNonQuery("USE master");
            database.ExecuteNonQuery("DROP DATABASE[{0}]", DatabaseName);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    SqlOperation($"stop {Instance}");
                    SqlOperation($"delete {Instance}");

                    string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                    File.Delete($"{homePath}/{DatabaseName}.mdf");
                    File.Delete($"{homePath}/{DatabaseName}_log.ldf");
                }

                this.disposedValue = true;
            }
        }

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

        private static void Connect(SqlServerDatabase database)
        {
            // Decrypt encrypted columns
            // Insert referential values
        }
    }
}
