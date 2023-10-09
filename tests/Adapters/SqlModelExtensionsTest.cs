// <copyright file="SqlModelExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Models;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;

    public class SqlModelExtensionsTest
    {
        [Fact]
        public void ToMandateDetail()
        {
            Guid id = Guid.NewGuid();
            Company company = new Company(Guid.NewGuid(), "mega", "45207964300014", "1999156874", string.Empty, null!);
            Bank? bank = new Bank("12345", "biap", "biap group", null!);
            Bban bban = new Bban("12345", "56789", "12345678901", "88", bank);
            Status status = new Status(CollectionStatus.ToDo, "todo");

            Collection collection = new Collection(
                id,
                company,
                bban,
                new DateTime(2023, 10, 1),
                new DateTime(2023, 10, 2),
                status);

            var model = collection.ToMandateDetail();

            var expected = new Client.MandateCollection(
                id,
                "1999156874",
                "mega",
                "biap",
                "12345678901",
                new DateTime(2023, 10, 1),
                new DateTime(2023, 10, 2),
                (int)CollectionStatus.ToDo,
                "todo");

            model.Should().BeEquivalentTo(expected);
        }
    }
}
