// <copyright file="CompanyNotFoundExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    using Newtonsoft.Json;

    public class CompanyNotFoundExceptionTest
    {
        [Fact]
        public void Constructor_Empty()
        {
            var ex = new CompanyNotFoundException();
            ex.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.Sql.CompanyNotFoundException' was thrown.");
        }

        [Fact]
        public void Constructor_Message()
        {
            var ex = new CompanyNotFoundException("message");
            ex.Message.Should().Be("message");
        }

        [Fact]
        public void Constructor_Nested()
        {
            var nested = new Exception("nested");
            var ex = new CompanyNotFoundException("message", nested);
            ex.Message.Should().Be("message");
            ex.InnerException.Should().Be(nested);
        }

        [Fact]
        public void Constructor_Serialize()
        {
            var exToSerialize = new CompanyNotFoundException("message", new Exception("nested"));

            var serializedEx = JsonConvert.SerializeObject(exToSerialize);
            var deserializedEx = JsonConvert.DeserializeObject<CompanyNotFoundException>(serializedEx);

            deserializedEx.Should().NotBeNull();
            deserializedEx?.Should().BeEquivalentTo(exToSerialize);
        }

        [Fact]
        public void Constructor_Custom_FromId()
        {
            var ex = CompanyNotFoundException.FromId("12345");

            ex.Message.Should().Be("La company avec l'id '12345' n'a pas été trouvée dans le référentiel");
        }

        [Fact]
        public void Constructor_Custom_FromSiret()
        {
            var ex = CompanyNotFoundException.FromSiret("12345123456789");

            ex.Message.Should().Be("La company avec le siret '12345123456789' n'a pas été trouvée dans le référentiel");
        }
    }
}
