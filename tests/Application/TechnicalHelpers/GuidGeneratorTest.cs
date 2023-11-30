// <copyright file="GuidGeneratorTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    public class GuidGeneratorTest
    {
        [Fact]
        public void NewGuid_GenerateNewId()
        {
            var generator = new GuidGenerator();
            var newId = generator.NewGuid();

            newId.Should().NotBeEmpty();
        }
    }
}
