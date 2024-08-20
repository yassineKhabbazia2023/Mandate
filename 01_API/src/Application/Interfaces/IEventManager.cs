// <copyright file="IEventManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces
{
    /// <summary>
    /// IEventManager.
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// Publishes a mandate creation event.
        /// </summary>
        /// <param name="message">The message to be published.</param>
        /// <param name="correlationId">The correlation ID for the message (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task PublishCreateMandateAsync(MandateCreationMessage message, string? correlationId = null);
    }
}