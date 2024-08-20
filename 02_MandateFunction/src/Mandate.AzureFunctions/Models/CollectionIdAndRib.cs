// <copyright file="CollectionIdAndRib.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Models
{
    public record CollectionIdAndRib(Guid CollectionId, Bban Bban);
}
