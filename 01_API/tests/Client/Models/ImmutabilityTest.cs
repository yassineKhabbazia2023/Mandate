// <copyright file="ImmutabilityTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    using System.Reflection;

    public class ImmutabilityTest
    {
        [Fact]
        public void AllImmutable()
        {
            var types = typeof(Company).Assembly.GetTypes().Where(a => a.IsClass && a.Namespace == "KPMG.Pulse.Back.Accounting.Mandate.Client");
            foreach (var type in types)
            {
                var props = type.GetProperties();
                foreach (var property in props)
                {
                    if (property.CanWrite)
                    {
                        throw new Exception($"{type} - {property} is mutable");
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
