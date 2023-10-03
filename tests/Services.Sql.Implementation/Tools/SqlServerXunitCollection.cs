// <copyright file="SqlServerXunitCollection.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    [CollectionDefinition("SerialExecutionPublishDb")]
    public class SqlServerXunitCollection : ICollectionFixture<SqlServerFixture>
    {
    }
}
