// <copyright file="FormIOAuthToken.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    public class FormioAuthToken
    {
        public FormioTokenType Type { get; set; }

        public string Value { get; set; } = null!;
    }
}
