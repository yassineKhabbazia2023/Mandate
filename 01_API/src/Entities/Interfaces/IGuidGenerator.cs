// <copyright file="IGuidGenerator.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IGuidGenerator
    {
        /// <summary>
        /// Generates a new guid.
        /// </summary>
        /// <returns>The generated guid.</returns>
        Guid NewGuid();
    }
}
