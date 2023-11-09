// <copyright file="XmlExtension.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using System.Xml.Serialization;

    public static class XmlExtension
    {
        public static string Serialize<T>(this T source)
        {
            if (source!.Equals(default(T)))
            {
                return string.Empty;
            }

            using var ms = new MemoryStream();
            var ser = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            ns.Add(string.Empty, string.Empty);
            ser.Serialize(ms, source, ns);
            ms.Position = 0;

            using var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static T Deserialize<T>(this string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using TextReader textReader = new StringReader(xml);

            return (T)serializer.Deserialize(textReader) !;
        }
    }
}
