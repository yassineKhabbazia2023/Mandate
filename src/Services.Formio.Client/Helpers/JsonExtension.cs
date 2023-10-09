// <copyright file="JsonExtension.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using Newtonsoft.Json;

    public static class JsonExtension
    {
        public static string Serialize<T>(this T source)
        {
            if (source == null)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(source);
        }

        public static T Deserialize<T>(this string jsonString)
        {
            //JsonConvert.DeserializeObject<T>(serializedSubject);
            return JsonConvert.DeserializeObject<T>(jsonString) !;
        }
    }
}
