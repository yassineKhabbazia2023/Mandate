// <copyright file="GuidGenerator.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class GuidGenerator : IGuidGenerator
    {
        /// <inheritdoc />
        public Guid NewGuid() => Guid.NewGuid();
    }
}
