// <copyright file="Signatory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class Signatory
    {
        [JsonConstructor]
        public Signatory(string? title, string? firstName, string? lastName, string? email)
        {
            this.Title = title;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
        }

        [JsonProperty("title")]
        public string? Title { get; }

        [JsonProperty("firstName")]
        public string? FirstName { get; }

        [JsonProperty("lastName")]
        public string? LastName { get; }

        [JsonProperty("email")]
        public string? Email { get; }
    }
}
