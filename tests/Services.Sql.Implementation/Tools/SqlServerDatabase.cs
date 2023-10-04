// <copyright file="SqlServerDatabase.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using System;
    using System.Data;
    using System.Globalization;
    using Microsoft.Data.SqlClient;

    internal sealed class SqlServerDatabase : IAsyncDisposable
    {
        private SqlConnection connection;

        internal SqlServerDatabase(string connectionString)
        {
            this.connection = new SqlConnection(connectionString);
            this.connection.Open();
        }

        public async ValueTask DisposeAsync()
        {
            if (this.connection != null)
            {
                await SqlServerFixture.DropDatabase(this).ConfigureAwait(false);
                this.connection.Dispose();
                this.connection = null!;
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string sqlCommand, params SqlParameter[] arguments)
        {
            using var command = this.connection.CreateCommand();
            command.CommandText = sqlCommand;
            command.Parameters.AddRange(arguments);
            return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        }

        public DataTable ExecuteQuery(string query)
        {
            using var adapter = new SqlDataAdapter(query, this.connection);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }
    }
}
