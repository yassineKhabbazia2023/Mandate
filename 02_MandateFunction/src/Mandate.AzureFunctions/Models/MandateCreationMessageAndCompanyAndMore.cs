// <copyright file="MandateCreationMessageAndCompanyAndMore.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using KPMG.Pulse.Back.Accounting.Mandate.Application;

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Models
{

    public record MandateCreationMessageAndCompanyAndMore(MandateCreationMessage MandateCreationMessage, Company Company, CollectionIdAndRib CollectionIdAndRib);
}
