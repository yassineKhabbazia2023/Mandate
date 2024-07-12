// <copyright file="SignatoryDetails.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class SignatoryDetails
    {
        public SignatoryDetails(string? collaboratorEmail, string? signatoryName, string? siretNumber)
        {
            this.CollaboratorEmail = collaboratorEmail;
            this.SignatoryName = signatoryName;
            this.SiretNumber = siretNumber;
        }

        public string? CollaboratorEmail { get; }

        public string? SignatoryName { get; }

        public string? SiretNumber { get; }
    }
}
