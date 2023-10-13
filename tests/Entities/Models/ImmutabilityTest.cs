// <copyright file="ImmutabilityTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    using System.Reflection;

    public class ImmutabilityTest
    {
        [Fact]
        public void AllImmutable()
        {
            var types = typeof(Company).Assembly.GetTypes().Where(a => a.IsClass && a.Namespace == "KPMG.Pulse.Back.Accounting.Mandate");
            foreach (var type in types)
            {
                var props = type.GetProperties();
                foreach (var property in props)
                {
                    if (property.CanWrite)
                    {
                        throw new Exception($"{type} - {property} is not immutable");
                    }
                }

                if (type.GetProperties().Any())
                {
                    type.GetProperties().Should().AllSatisfy(p => p.CanWrite.Should().BeFalse());
                }
            }
        }
    }
}
