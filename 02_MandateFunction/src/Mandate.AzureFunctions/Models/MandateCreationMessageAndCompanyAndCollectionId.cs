// <copyright file="MandateCreationMessageAndCompany.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Models
{
    using KPMG.Pulse.Back.Accounting.Mandate.Application;

    public record MandateCreationMessageAndCompanyAndCollectionId(MandateCreationMessage MandateCreationMessage, Company Company, Guid CollectionId);
}
