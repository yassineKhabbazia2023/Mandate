// <copyright file="SqlServerDatabase.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using System;
    using System.Data;
    using System.Globalization;
    using Microsoft.Data.SqlClient;

    internal sealed class SqlServerDatabase : IDisposable
    {
        private readonly string connectionString;
        private SqlConnection connection;

        internal SqlServerDatabase(string connectionString)
        {
            this.connectionString = connectionString;
            this.connection = new SqlConnection(connectionString);
            this.connection.Open();
        }

        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
        }

        public void Dispose()
        {
            if (this.connection != null)
            {
                SqlServerFixture.DropDatabase(this);
                this.connection.Dispose();
                this.connection = null!;
            }
        }

        public int ExecuteNonQuery(string sqlCommand, params object[] arguments)
        {
            using var command = this.connection.CreateCommand();
            command.CommandText = string.Format(CultureInfo.InvariantCulture, sqlCommand, arguments);
            return command.ExecuteNonQuery();
        }

        public async Task<int> ExecuteNonQueryAsync(string sqlCommand, params object[] arguments)
        {
            using var command = this.connection.CreateCommand();
            command.CommandText = string.Format(CultureInfo.InvariantCulture, sqlCommand, arguments);
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
