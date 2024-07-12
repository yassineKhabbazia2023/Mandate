// <copyright file="FluentAssertionsExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    using FluentAssertions.Json;
    using FluentAssertions.Primitives;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public static class FluentAssertionsExtensions
    {
        public static void BeJsonSerializableTo(this ObjectAssertions objectAssertions, object expected)
        {
            var jsonSubject = JToken.FromObject(objectAssertions.Subject);
            var jsonExpected = JToken.FromObject(expected);
            JsonAssertionExtensions.Should(jsonSubject).BeEquivalentTo(jsonExpected);
        }

        public static void BeJsonSerializableTo(this ObjectAssertions objectAssertions, string expected)
        {
            var jsonSubject = JToken.FromObject(objectAssertions.Subject);
            var jsonExpected = JToken.Parse(expected);
            JsonAssertionExtensions.Should(jsonSubject).BeEquivalentTo(jsonExpected);
        }

        public static void BeJsonDeserializableTo<T>(this ObjectAssertions objectAssertions, T expected)
        {
            var serializedSubject = JsonConvert.SerializeObject(objectAssertions.Subject);
            var deserializedSubject = JsonConvert.DeserializeObject<T>(serializedSubject);
            deserializedSubject.Should().BeEquivalentTo(expected);
        }
    }
}
