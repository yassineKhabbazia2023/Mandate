// <copyright file="BankCodeNotFoundExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    using Newtonsoft.Json;

    public class BankCodeNotFoundExceptionTest
    {
        [Fact]
        public void Constructor_Empty()
        {
            var ex = new BankCodeNotFoundException();
            ex.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.Sql.BankCodeNotFoundException' was thrown.");
        }

        [Fact]
        public void Constructor_Message()
        {
            var ex = new BankCodeNotFoundException("message");
            ex.Message.Should().Be("message");
        }

        [Fact]
        public void Constructor_Nested()
        {
            var nested = new Exception("nested");
            var ex = new BankCodeNotFoundException("message", nested);
            ex.Message.Should().Be("message");
            ex.InnerException.Should().Be(nested);
        }

        [Fact]
        public void Constructor_Serialize()
        {
            var exToSerialize = new BankCodeNotFoundException("message", new Exception("nested"));

            var serializedEx = JsonConvert.SerializeObject(exToSerialize);
            var deserializedEx = JsonConvert.DeserializeObject<BankCodeNotFoundException>(serializedEx);

            deserializedEx.Should().NotBeNull();
            deserializedEx?.Should().BeEquivalentTo(exToSerialize);
        }

        [Fact]
        public void Constructor_Custom()
        {
            var ex = BankCodeNotFoundException.FromId("12345");

            ex.Message.Should().Be("La banque avec le code '12345' n'a pas été trouvée dans le référentiel");
        }
    }
}
